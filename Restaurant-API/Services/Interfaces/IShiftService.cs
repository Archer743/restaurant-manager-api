using Restaurant_API.Data.Entities;
using Restaurant_API.Data.Models.Shift;

namespace Restaurant_API.Services.Interfaces
{
    public interface IShiftService
    {
        Task<Shift> GetByIdAsync(Guid shiftId);

        Task<List<Shift>> GetRestaurantShiftsAsync(Guid restaurantId);

        Task<ShiftContext> GetShiftContextAsync(Reservation reservation);

        /// <returns>A dictionary where the key is the table's ID and the value is the ID of the reservation currently occupying that table.</returns>
        Task<Dictionary<Guid, Guid>> GetReservedTablesForShiftAsync(Shift shift, DateOnly localDate, TimeZoneInfo restaurantTimeZone);

        Task<bool> HasShiftAtTimeAsync(Restaurant restaurant, DateTime dateTimeUtc);

        Task ValidateReservationShiftChangeAsync(Reservation reservation, DateTime newStartTimeUtc);
    }
}
