using Restaurant_API.Constants;
using Restaurant_API.Data.Entities;
using Restaurant_API.Exceptions;
using Restaurant_API.Extensions;
using Restaurant_API.Repositories.Interfaces;
using Restaurant_API.Services.Interfaces;

namespace Restaurant_API.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepo restaurantRepo;
        private readonly IRestaurantUserRepo restaurantUserRepo;

        public RestaurantService(IRestaurantRepo restaurantRepo, IRestaurantUserRepo restaurantUserRepo)
        {
            this.restaurantRepo = restaurantRepo;
            this.restaurantUserRepo = restaurantUserRepo;
        }

        public async Task<Restaurant> GetByIdAsync(Guid restaurantId)
        {
            return await restaurantRepo.GetByIdAsync(restaurantId) ??
                                        throw new NotFoundException(ErrorCodes.Restaurant.NotFound);
        }

        public async Task<Restaurant> GetCurrUserAssociatedRestaurantAsync(HttpContext ctx)
        {
            User currUser = ctx.GetCurrUser();

            // Retrieve the restaurant the user is associated with (staff members must be linked to a restaurant)
            Restaurant restaurant = await restaurantUserRepo.GetRestaurantByUserIdAsync(currUser.Id) ??
                                    throw new BadRequestException(ErrorCodes.User.NoAssociatedRestaurant);

            return restaurant;
        }

        public TimeZoneInfo GetTimeZoneInfo(Restaurant restaurant)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(restaurant.TimeZoneId);
        }
    }
}
