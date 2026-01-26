using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Settings
{
	public class ReservationCleanup
	{
		[Required]
		public uint RetentionMonths { get; init; }
	}
}
