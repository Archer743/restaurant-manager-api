using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Settings
{
	public class SessionSettings
	{
		[Required]
		public uint RefreshTokenLength { get; init; }

		[Required]
		public string ServerSecretKey { get; init; }

		[Required]
		public TimeSpan SessionLifetime { get; init; }

		[Required]
		public uint RetentionDays { get; init; }
	}
}
