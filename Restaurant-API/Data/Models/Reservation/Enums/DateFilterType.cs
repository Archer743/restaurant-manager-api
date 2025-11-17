namespace Restaurant_API.Data.Models.Reservation.Enums
{
    public enum DateFilterType
    {
        Today,
        Tomorrow,
        Upcoming,  // (Next 7 days)
        All,
        Custom,
    }
}
