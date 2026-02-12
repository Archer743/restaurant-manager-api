using Restaurant_API.Constants;
using Restaurant_API.Data.Entities;
using Restaurant_API.Data.Models.Shift;
using Restaurant_API.Exceptions;
using Restaurant_API.Repositories.Interfaces;
using Restaurant_API.Services.Interfaces;

namespace Restaurant_API.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IRestaurantService restaurantService;
        private readonly ITimeZoneService timeZoneService;

        private readonly IShiftRepo shiftRepo;
        private readonly IReservationRepo reservationRepo;

        public ShiftService(IRestaurantService restaurantService, ITimeZoneService timeZoneService, IShiftRepo shiftRepo, IReservationRepo reservationRepo)
        {
            this.restaurantService = restaurantService;
            this.timeZoneService = timeZoneService;

            this.shiftRepo = shiftRepo;
            this.reservationRepo = reservationRepo;
        }

        public async Task<Shift> GetByIdAsync(Guid shiftId)
        {
            return await shiftRepo.GetByIdAsync(shiftId) ??
            throw new NotFoundException(ErrorCodes.Shift.NotFound);
        }

        public async Task<List<Shift>> GetRestaurantShiftsAsync(Guid restaurantId)
        {
            return await shiftRepo.GetRestaurantShiftsAsync(restaurantId);
        }

        public async Task<ShiftContext> GetShiftContextAsync(Reservation reservation)
        {
            return await GetShiftContextAsync(reservation.Restaurant, reservation.StartUtc) ??
            throw new NotFoundException(ErrorCodes.Reservation.OutsideShift);
        }

        public async Task<Dictionary<Guid, Guid>> GetReservedTablesForShiftAsync(Shift shift, DateOnly localDate, TimeZoneInfo restaurantTimeZone)
        {
            var (shiftStartUtc, shiftEndUtc) = CalculateShiftPeriodUtc(shift, localDate, restaurantTimeZone);
            return await reservationRepo.GetReservedTablesInPeriodAsync(shift.RestaurantId, shiftStartUtc, shiftEndUtc);
        }

        public async Task<bool> HasShiftAtTimeAsync(Restaurant restaurant, DateTime dateTimeUtc)
        {
            return await GetShiftAtTimeAsync(restaurant, dateTimeUtc) != null;
        }

        public async Task ValidateReservationShiftChangeAsync(Reservation reservation, DateTime newStartUtc)
        {
            Shift? currShift = await GetShiftAtTimeAsync(reservation.Restaurant, reservation.StartUtc);
            ShiftContext? newShiftCtx = await GetShiftContextAsync(reservation.Restaurant, newStartUtc);

            if (newShiftCtx == null)
            {
                throw new BadRequestException(ErrorCodes.Reservation.StartTimeOutsideShift);
            }

            // No need to check conflicts if it's the same shift
            if (currShift != null && currShift.Id == newShiftCtx.Shift.Id && reservation.StartUtc.Date == newStartUtc.Date) return;

            var currTableIds = reservation.Tables.Select(t => t.Id).ToHashSet();
            var conflicts = currTableIds.Intersect(newShiftCtx.ReservedTableIds).ToList();

            if (conflicts.Any())
            {
                throw new ConflictException(ErrorCodes.Table.ReservedInNewShift);
            }
        }

        /// <summary> Calculate shift start and end in UTC for a specific date</summary>
        private (DateTime shiftStartUtc, DateTime shiftEndUtc) CalculateShiftPeriodUtc(Shift shift, DateOnly localDate, TimeZoneInfo restaurantTimeZone)
        {
            var shiftStartUtc = TimeZoneInfo.ConvertTimeToUtc(localDate.ToDateTime(shift.StartTime), restaurantTimeZone);
            var shiftEndUtc = TimeZoneInfo.ConvertTimeToUtc(localDate.ToDateTime(shift.EndTime), restaurantTimeZone);

            return (shiftStartUtc, shiftEndUtc);
        }

        private async Task<ShiftContext?> GetShiftContextAsync(Restaurant restaurant, DateTime dateTimeUtc)
        {
            var restaurantTimeZone = restaurantService.GetTimeZoneInfo(restaurant);
            var (localDate, localTime) = timeZoneService.ConvertToLocalDateTimeParts(restaurantTimeZone, dateTimeUtc);

            var shift = await shiftRepo.GetRestaurantShiftAtTimeAsync(restaurant.Id, localTime);
            if (shift == null) return null;

            // Get all table IDs reserved in this shift
            var (shiftStartUtc, shiftEndUtc) = CalculateShiftPeriodUtc(shift, localDate, restaurantTimeZone);
            var reservedTableIds = await reservationRepo.GetReservedTableIdsInPeriodAsync(shift.RestaurantId, shiftStartUtc, shiftEndUtc);

            return new ShiftContext
            {
                Shift = shift,
                ReservedTableIds = reservedTableIds,
            };
        }

        private async Task<Shift?> GetShiftAtTimeAsync(Restaurant restaurant, DateTime dateTimeUtc)
        {
            var (_, localTime) = timeZoneService.ConvertToLocalDateTimeParts(restaurant, dateTimeUtc);
            return await shiftRepo.GetRestaurantShiftAtTimeAsync(restaurant.Id, localTime);
        }
    }
}
