using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_API.Data.Entities
{
	public class User
	{
		[Key]
		public Guid Id { get; set; }


		// Unique
		[Required]
		[StringLength(40, MinimumLength = 3)]
		public string Username { get; set; } = string.Empty;


        [Required]
		[StringLength(50, MinimumLength = 1)]
		public string FirstName { get; set; } = string.Empty;


        [Required]
		[StringLength(50, MinimumLength = 1)]
		public string LastName { get; set; } = string.Empty;


        [Required]
		public string PasswordHash { get; set; } = string.Empty;


        [ForeignKey(nameof(Role))]
		public Guid RoleId { get; set; }

		public Role Role { get; set; }
	}
}
