using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Data.Entities
{
	public class Session
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }


		[ForeignKey(nameof(User))]
		public Guid UserId { get; set; }

		public User User { get; set; }


        // Unique
        [Required]
		public string RefreshTokenHash { get; set; }


		public DateTime CreatedAt { get; set; }


		public DateTime ExpiresAt { get; set; }


		public bool IsRevoked { get; set; }
	}
}
