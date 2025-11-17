using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Data.Entities;
using Restaurant_API.Repositories.Interfaces;

namespace Restaurant_API.Repositories
{
    public class RestaurantUserRepo : IRestaurantUserRepo
    {
        private readonly AppDbContext context;

        public RestaurantUserRepo(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<Restaurant?> GetRestaurantByUserIdAsync(Guid userId)
        {
            return await context.RestaurantUsers
                        .Where(ru => ru.UserId == userId)
                        .Include(ru => ru.Restaurant)
                        .Select(ru => ru.Restaurant)
                        .FirstOrDefaultAsync();
        }
    }
}
