using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_API.Data.Entities
{
    public class Shift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        [ForeignKey(nameof(Restaurant))]
        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;


        [MaxLength(100)]
        public string Name { get; set; } = null!;


        public TimeOnly StartTime { get; set; }


        public TimeOnly EndTime { get; set; }
    }
}
