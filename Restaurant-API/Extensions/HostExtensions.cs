using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Data.Entities;

namespace Restaurant_API.Extensions
{
	public static class HostExtensions
    {
        public static async Task MigrateAndSeedDbAsync(this IHost host)
        {
			using var scope = host.Services.CreateScope();
			var services = scope.ServiceProvider;
			
			var logger = services.GetRequiredService<ILogger<IHost>>();
			var dbContext = services.GetRequiredService<AppDbContext>();

			try
			{
				logger.LogInformation("==================================== Starting database migration...");

				// 1. Apply Migrations
				await dbContext.Database.MigrateAsync();
				logger.LogInformation("==================================== Database migration completed successfully.");

				// 2. Seed Database
				await SeedRequiredDataAsync(dbContext);
				logger.LogInformation("==================================== Database seeding completed successfully.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "==================================== An error occurred while migrating or seeding the database.");
				// Throwing the exception ensures the migrator container fails with a non-zero exit code.
				throw;
			}
		}
        
        private static async Task SeedRequiredDataAsync(AppDbContext context)
        {
            // Seed roles
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { Name = "Staff", NormalizedName = "STAFF" },
                    new Role { Name = "Admin", NormalizedName = "ADMIN" }
                );
                await context.SaveChangesAsync();
			}

            // Seed restaurants
			if (!await context.Restaurants.AnyAsync())
			{
				context.Restaurants.AddRange(
					new Restaurant { Name = "Izbata Tavern", TimeZoneId = "Europe/Sofia" },
					new Restaurant { Name = "Izbata Tavern 2", TimeZoneId = "Europe/Sofia" }
				);
				await context.SaveChangesAsync();
			}

			// TODO: Seed an admin user and add them to the restaurants
        }
    }
}