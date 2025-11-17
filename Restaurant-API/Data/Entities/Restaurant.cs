using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_API.Data.Entities
{
    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;


        /// <summary>
        /// Unique identifier for the time zone in which this restaurant operates.
        /// This ID is crucial for correctly converting UTC stored times to the restaurant's local time.
        /// It should be an IANA Time Zone Database (TZDB) identifier
        /// (e.g., "Europe/Sofia", "America/New_York") for cross-platform compatibility and proper handling of daylight saving time.
        /// </summary>
        [Required]
        public string TimeZoneId { get; set; }
    }
}
