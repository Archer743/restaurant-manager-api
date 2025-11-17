using Restaurant_API.Services.Interfaces;
using Restaurant_API.Data.Entities;

namespace Restaurant_API.Services
{
    public class TimeZoneService : ITimeZoneService
    {
        private readonly IRestaurantService restaurantService;

        public TimeZoneService(IRestaurantService restaurantService)
        {
            this.restaurantService = restaurantService;
        }

        public (DateOnly localDate, TimeOnly localTime) ConvertToLocalDateTimeParts(Restaurant restaurant, DateTime dateTimeUtc)
        {
            var timeZone = restaurantService.GetTimeZoneInfo(restaurant);
            return ConvertToLocalDateTimeParts(timeZone, dateTimeUtc);
        }

        public (DateOnly localDate, TimeOnly localTime) ConvertToLocalDateTimeParts(TimeZoneInfo timeZone, DateTime dateTimeUtc)
        {
            var dateTimeLocal = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, timeZone);
            return (DateOnly.FromDateTime(dateTimeLocal), TimeOnly.FromDateTime(dateTimeLocal));
        }
    }
}
