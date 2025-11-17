namespace Restaurant_API.Data.Models.Reservation
{
    public class ReservationViewModel
    {
        public Guid Id { get; set; }

        public string Tag { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DateTime StartTimeUtc { get; set; }

        public int PeopleCount { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Comment { get; set; }

        public bool IsCreatedByStaff { get; set; }

        public bool IsClosed { get; set; }
    }
}
