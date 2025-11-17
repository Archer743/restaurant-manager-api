using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Data.Entities;
using Restaurant_API.Repositories.Interfaces;

namespace Restaurant_API.Repositories
{
    public class RestaurantRepo : IRestaurantRepo
    {
        private readonly AppDbContext context;

        public RestaurantRepo(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<Restaurant?> GetByIdAsync(Guid id)
        {
            return await context.Restaurants
                                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
