using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Data.Entities;
using Restaurant_API.Repositories.Interfaces;

namespace Restaurant_API.Repositories
{
    public class ShiftRepo : IShiftRepo
    {
        private readonly AppDbContext context;

        public ShiftRepo(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<Shift?> GetByIdAsync(Guid id)
        {
            return await context.Shifts
                                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Shift?> GetRestaurantShiftAtTimeAsync(Guid restaurantId, TimeOnly localTime)
        {
            return await context.Shifts
                                .FirstOrDefaultAsync(s => s.RestaurantId == restaurantId &&
                                                          localTime >= s.StartTime && localTime < s.EndTime);
        }

        public async Task<List<Shift>> GetRestaurantShiftsAsync(Guid restaurantId)
        {
            return await context.Shifts
                                .Where(s => s.RestaurantId == restaurantId)
                                .OrderBy(s => s.StartTime)
                                .ToListAsync();
        }
    }
}
