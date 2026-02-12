using Restaurant_API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Data.Models.Other
{
	public class UtcRangeQuery
	{
		[Required, MustBeUtc]
		public DateTime StartUtc { get; init; }

		[Required, MustBeUtc]
		public DateTime EndUtc { get; init; }

		public (DateTime Start, DateTime End) Normalize()
		{
			return StartUtc <= EndUtc ? (StartUtc, EndUtc) : (EndUtc, StartUtc);
		}
	}
}
