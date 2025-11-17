using Restaurant_API.Data.Entities;

namespace Restaurant_API.Services.Interfaces
{
    public interface ITimeZoneService
    {
        (DateOnly localDate, TimeOnly localTime) ConvertToLocalDateTimeParts(Restaurant restaurant, DateTime dateTimeUtc);

        (DateOnly localDate, TimeOnly localTime) ConvertToLocalDateTimeParts(TimeZoneInfo timeZone, DateTime dateTimeUtc);
    }
}
