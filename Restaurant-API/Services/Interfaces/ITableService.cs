using Microsoft.AspNetCore.Mvc;
using Restaurant_API.Data.Models.Shift;
using Restaurant_API.Data.Models.Table;

namespace Restaurant_API.Services.Interfaces
{
    public interface ITableService
    {
        Task<FloorPlanViewModel> GetRestaurantFloorPlanAsync(HttpContext ctx, DateOnly date, Guid? shiftId);

        Task<ReservationShiftViewModel> GetTablesAvailabilityForReservationAsync(HttpContext ctx, Guid reservationId);

        Task<ReservationShiftViewModel?> UpdateReservationTablesAsync(HttpContext ctx, Guid reservationId, ReservationTablesUpdate updateModel);
    }
}
