using Restaurant_API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Data.Models.Reservation
{
    public class ReservationCreateModel
    {
        // Name of the client
        [MaxLength(100)]
        public string Name { get; set; }

        [MustBeUtc]
        public DateTime StartTimeUtc { get; set; }

        [Range(1, 100)]
        public int PeopleCount { get; set; }

        // Optional data
        [OptionalEmail]
        public string? Email { get; set; }

        [OptionalPhone]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
