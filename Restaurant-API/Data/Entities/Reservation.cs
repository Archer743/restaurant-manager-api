using Restaurant_API.Data.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_API.Data.Entities
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        [ForeignKey(nameof(Restaurant))]
        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }


        // A user-friendly, unique identifier for a reservation within a specific restaurant.
        // A tag can exist in multiple restaurants, but must be unique for each restaurant.
        [Required]
        public string Tag { get; set; }


        // Required name of the client (TODO: change to ClientName)
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;


        [Required]
        public DateTime StartUtc { get; set; }


        [Required, Range(1, 100)]
        public int PeopleCount { get; set; }


        // Optional data
        public string? Email { get; set; }


        public string? Phone { get; set; }


        [MaxLength(500)]
        public string? Comment { get; set; }


        public ReservationState State { get; set; }


        // Indicates if this reservation was created directly by a staff member
        public bool IsCreatedByStaff { get; set; } = false;


        public bool IsClosed { get; set; } = false;


        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;


        public ICollection<Table> Tables { get; set; } = [];
    }
}
