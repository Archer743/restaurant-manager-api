using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Data.Entities;
using Restaurant_API.Repositories.Interfaces;

namespace Restaurant_API.Repositories
{
    public class TableRepo : ITableRepo
    {
        private readonly AppDbContext context;

        public TableRepo(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Table>> GetAllRestaurantTablesAsync(Guid restaurantId)
        {
            return await context.Tables
                                .Where(t => t.RestaurantId == restaurantId)
                                .OrderBy(t => t.TableNumber)
                                .ToListAsync();
        }

        public async Task<List<Table>> GetTablesByIdsAsync(HashSet<Guid> tableIds)
        {
            return await context.Tables
                                .Where(t => tableIds.Contains(t.Id))
                                .ToListAsync();
        }
    }
}
