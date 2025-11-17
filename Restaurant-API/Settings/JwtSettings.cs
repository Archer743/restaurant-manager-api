using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Settings
{
	public class JwtSettings
	{
		[Required]
		public string Issuer { get; init; }

		[Required]
		public string Audience { get; init; }

		[Required]
		public string ServerSecretKey { get; init; }

		[Required]
		public TimeSpan AccessTokenLifetime { get; init; }
	}
}
