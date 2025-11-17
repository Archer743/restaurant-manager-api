using Restaurant_API.Data.Entities;

namespace Restaurant_API.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<Restaurant> GetByIdAsync(Guid restaurantId);

        Task<Restaurant> GetCurrUserAssociatedRestaurantAsync(HttpContext ctx);

        TimeZoneInfo GetTimeZoneInfo(Restaurant restaurant);
    }
}
