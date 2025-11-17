using Restaurant_API.Data.Entities;

namespace Restaurant_API.Repositories.Interfaces
{
    public interface IShiftRepo
    {
        Task<Shift?> GetByIdAsync(Guid id);

        Task<Shift?> GetRestaurantShiftAtTimeAsync(Guid restaurantId, TimeOnly localTime);

        Task<List<Shift>> GetRestaurantShiftsAsync(Guid restaurantId);
    }
}
