namespace Restaurant_API.Data.Models.Reservation
{
    public class FloorPlanReservationPreviewModel
    {
        public Guid Id { get; set; }

        public string Tag { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        // Local time of day based on the restaurant time zone
        public TimeOnly Time { get; set; }

        public int PeopleCount { get; set; }

        public int TableCount { get; set; }
    }
}
