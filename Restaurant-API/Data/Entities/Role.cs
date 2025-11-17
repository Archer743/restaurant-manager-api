using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_API.Data.Entities
{
	public class Role
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }


		[Required]
		[StringLength(40, MinimumLength = 1)]
		public string Name { get; set; }


		// Unique
		[Required]
		[StringLength(40, MinimumLength = 1)]
		public string NormalizedName { get; set; }
	}
}
