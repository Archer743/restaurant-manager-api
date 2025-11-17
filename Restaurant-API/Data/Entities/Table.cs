using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Data.Entities
{
    public class Table
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        [ForeignKey(nameof(Restaurant))]
        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }


        // Unique for the restaurant. Used as user-friendly id inside the restaurant
        public int TableNumber { get; set; }


        // People capacity
        public int Capacity { get; set; }


        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
