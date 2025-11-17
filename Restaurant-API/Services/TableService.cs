using Restaurant_API.Constants;
using Restaurant_API.Data.Entities;
using Restaurant_API.Data.Mappers;
using Restaurant_API.Data.Models.Shift;
using Restaurant_API.Data.Models.Table;
using Restaurant_API.Exceptions;
using Restaurant_API.Repositories.Interfaces;
using Restaurant_API.Services.Interfaces;

namespace Restaurant_API.Services
{
    public class TableService : ITableService
    {
        private readonly IReservationService reservationService;
        private readonly IRestaurantService restaurantService;
        private readonly IShiftService shiftService;
        private readonly ITimeZoneService timeZoneService;
        private readonly ITableRepo tableRepo;
        private readonly IReservationRepo reservationRepo;

        public TableService(IReservationService reservationService, IRestaurantService restaurantService, IShiftService shiftService, ITimeZoneService timeZoneService, ITableRepo tableRepo, IReservationRepo reservationRepo)
        {
            this.reservationService = reservationService;
            this.restaurantService = restaurantService;
            this.shiftService = shiftService;
            this.timeZoneService = timeZoneService;

            this.tableRepo = tableRepo;
            this.reservationRepo = reservationRepo;
        }

        public async Task<FloorPlanViewModel> GetRestaurantFloorPlanAsync(HttpContext ctx, DateOnly date, Guid? shiftId)
        {
            var currUserRestaurant = await restaurantService.GetCurrUserAssociatedRestaurantAsync(ctx);
            var restaurantTimeZone = restaurantService.GetTimeZoneInfo(currUserRestaurant);

            List<Shift>? shifts = null;
            int selectedShiftIndex = 0;
            Shift selectedShift;

            if (shiftId == null)
            {
                // Get all restaurant's shifts sorted by start time
                shifts = await shiftService.GetRestaurantShiftsAsync(currUserRestaurant.Id);

                if (shifts.Count == 0)
                {
                    throw new NotFoundException(ErrorCodes.Restaurant.NoShiftsFound);
                }

                var (_, localTime) = timeZoneService.ConvertToLocalDateTimeParts(restaurantTimeZone, DateTime.UtcNow);

                // Loop through shifts to find the current shift or the next chronologically closest one
                for (int i = 0; i < shifts.Count; i++)
                {
                    if (localTime >= shifts[i].StartTime && localTime < shifts[i].EndTime)
                    {
                        selectedShiftIndex = i; break;
                    } else if (localTime < shifts[i].StartTime)
                    {
                        selectedShiftIndex = i; break;
                    }
                }

                selectedShift = shifts[selectedShiftIndex];
            } else
            {
                selectedShift = await shiftService.GetByIdAsync(shiftId.Value);

                if (selectedShift.RestaurantId != currUserRestaurant.Id)
                {
                    throw new ForbiddenException(ErrorCodes.Shift.BelongsToDifferentRestaurant);
                }
            }

            // =========================== Tables
            var tables = await tableRepo.GetAllRestaurantTablesAsync(currUserRestaurant.Id);
            var reservedTablesMap = await shiftService.GetReservedTablesForShiftAsync(selectedShift, date, restaurantTimeZone);

            // Set state for each table - Reserved / Available
            var tablesAvailability = tables.Select(table =>
            {
                var dto = table.ToFloorPlanAvailabilityView();

                if (reservedTablesMap.TryGetValue(table.Id, out Guid reservationId))
                {
                    dto.Status = TableStatus.Reserved;
                    dto.ReservationId = reservationId;
                } else
                {
                    dto.Status = TableStatus.Available;
                }
                    
                return dto;
            }).ToList();

            return new FloorPlanViewModel
            {
                Tables = tablesAvailability,
                Shifts = shifts?.Select(s => s.ToView()).ToList() ?? null,
                SelectedShiftIndex = shiftId == null ? selectedShiftIndex : null,
            };
        }

        public async Task<ReservationShiftViewModel> GetTablesAvailabilityForReservationAsync(HttpContext ctx, Guid reservationId)
        {
            // Load reservation and verify ownership
            Reservation reservation = await reservationService.GetReservationIfOwnedAsync(ctx, reservationId, includeDetails: true);

            if (reservation.IsClosed)
            {
                throw new ConflictException(ErrorCodes.Reservation.ClosedTablesUnavailable);
            }

            ShiftContext shiftCtx = await shiftService.GetShiftContextAsync(reservation);

            return await BuildReservationShiftViewAsync(reservation, shiftCtx);
        }
    
        public async Task<ReservationShiftViewModel?> UpdateReservationTablesAsync(HttpContext ctx, Guid reservationId, ReservationTablesUpdate updateModel)
        {
            // Load reservation and verify ownership
            Reservation reservation = await reservationService.GetReservationIfOwnedAsync(ctx, reservationId, includeDetails: true);

            if (reservation.IsClosed)
            {
                throw new ConflictException(ErrorCodes.Table.CannotModifyOfClosedReservation);
            }

            bool hasAdds = updateModel.TablesToAdd?.Any() ?? false;
            bool hasRemoves = updateModel.TablesToRemove?.Any() ?? false;

            if (!hasAdds && !hasRemoves)
                return null; // Nothing to update

            List<Table> reservationTables = reservation.Tables.ToList();
            ShiftContext shiftCtx;

            // Handle removal of tables
            if (hasRemoves)
            {
                var currentIds = reservationTables.Select(t => t.Id).ToHashSet();
                var toRemoveIds = updateModel.TablesToRemove!.ToHashSet();
                var missing = toRemoveIds.Except(currentIds).ToList();

                if (missing.Any())
                {
                    throw new BadRequestException(ErrorCodes.Table.NotPartOfReservation);
                }

                reservationTables.RemoveAll(t => toRemoveIds.Contains(t.Id));
            }

            // Handle adding new tables
            if (hasAdds)
            {
                var toAddIds = updateModel.TablesToAdd!.ToHashSet();

                var tablesToAdd = await tableRepo.GetTablesByIdsAsync(toAddIds);
                var foundIds = tablesToAdd.Select(t => t.Id).ToHashSet();

                var missing = toAddIds.Except(foundIds).ToList();

                if (missing.Any())
                {
                    throw new NotFoundException(ErrorCodes.Table.NotFound);
                }

                // Ensure all tables belong to same restaurant
                bool allSameRestaurant = tablesToAdd.All(t => t.RestaurantId == reservation.RestaurantId);
                if (!allSameRestaurant)
                {
                    throw new ConflictException(ErrorCodes.Table.BelongsToDifferentRestaurant);
                }

                // Ensure no conflicts in shift period - table already assigned to another reservation
                shiftCtx = await shiftService.GetShiftContextAsync(reservation);

                var conflictIds = toAddIds.Intersect(shiftCtx.ReservedTableIds).ToList();
                if (conflictIds.Any())
                {
                    throw new ConflictException(ErrorCodes.Table.AlreadyReserved);
                }

                reservationTables.AddRange(tablesToAdd);
            }

            // Apply the changes and persist
            reservation.Tables = reservationTables;
            await reservationRepo.UpdateAsync(reservation);

            // Update shift context
            shiftCtx = await shiftService.GetShiftContextAsync(reservation);

            return await BuildReservationShiftViewAsync(reservation, shiftCtx);
        }
    
        private async Task<ReservationShiftViewModel> BuildReservationShiftViewAsync(Reservation reservation, ShiftContext shiftContext)
        {
            // Get all restaurant tables
            var tables = await tableRepo.GetAllRestaurantTablesAsync(reservation.RestaurantId);

            // Get table IDs for this specific reservation
            var currReservationTableIds = reservation.Tables.Select(t => t.Id).ToHashSet();

            // Map each table to a DTO and set its status
            var tablesAvailability = tables.Select(table =>
            {
                var dto = table.ToAvailabilityView();

                if (currReservationTableIds.Contains(table.Id))
                    dto.Status = TableStatus.ReservedByCurrent;
                else if (shiftContext.ReservedTableIds.Contains(table.Id))
                    dto.Status = TableStatus.Reserved;
                else
                    dto.Status = TableStatus.Available;

                return dto;
            }).ToList();

            var shiftView = shiftContext.Shift.ToReservationView();
            shiftView.Tables = tablesAvailability;

            return shiftView;
        }
    }
}
