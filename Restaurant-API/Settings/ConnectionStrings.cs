using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Settings
{
	public class ConnectionStrings
	{
		[Required]
		public string PostgresConnection { get; init; }
	}
}
