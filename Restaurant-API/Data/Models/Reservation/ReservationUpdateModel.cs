using Restaurant_API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Data.Models.Reservation
{
    public class ReservationUpdateModel
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [OptionalEmail]
        public string? Email { get; set; }

        [OptionalPhone]
        public string? Phone { get; set; }

        [MustBeUtc]
        public DateTime? StartUtc { get; set; }

        [Range(1, 100)]
        public int? PeopleCount { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
