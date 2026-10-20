using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Settings
{
	public class ReservationCleanup
	{
		[Required, Range(1, 1200)]
		public uint RetentionMonths { get; init; }
	}
}
