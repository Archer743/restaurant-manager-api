namespace Restaurant_API.Data.Models.Reservation
{
    public class PendingReservationPreviewModel
    {
        public Guid Id { get; set; }

        public string Tag { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DateTime StartUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public int PeopleCount { get; set; }
    }
}
