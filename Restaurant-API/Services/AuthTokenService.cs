using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Restaurant_API.Services.Interfaces;
using Restaurant_API.Settings;
using System.Security.Cryptography;
using Restaurant_API.Data.Models.Auth;

namespace Restaurant_API.Services
{
    public class AuthTokenService : IAuthTokenService
	{
		private readonly JwtSettings jwtSettings;
		private readonly SessionSettings sessionSettings;

		private readonly TimeProvider timeProvider;

		public AuthTokenService(IOptions<JwtSettings> jwtSettings, IOptions<SessionSettings> sessionSettings, TimeProvider timeProvider)
		{
			this.jwtSettings = jwtSettings.Value;
			this.sessionSettings = sessionSettings.Value;
			this.timeProvider = timeProvider;
		}

		public UserAuthModel GenerateAuthTokens(Guid userId, Guid sessionId)
        {
			return new UserAuthModel
			{
				AccessToken = GenerateAccessToken(userId, sessionId),
				RefreshToken = GenerateRefreshToken()
			};
		}

        // Refresh token is a random string
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[sessionSettings.RefreshTokenLength];
            using var rng = RandomNumberGenerator.Create();  // Creates a cryptographically secure random number generator
            rng.GetBytes(randomBytes);  // Fills the randomBytes array with secure random values

            return Convert.ToBase64String(randomBytes);  // Base64 string is URL-safe
        }

        // Access Token is a JWT token
        private string GenerateAccessToken(Guid userId, Guid sessionId)
		{
			var expiration = timeProvider.GetUtcNow().UtcDateTime.Add(jwtSettings.AccessTokenLifetime);

			Claim[] claims = [
				new Claim("userId", userId.ToString()),
				new Claim("sessionId", sessionId.ToString()),
			];

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.ServerSecretKey));

			var token = new JwtSecurityToken(
				issuer: jwtSettings.Issuer,
				audience: jwtSettings.Audience,
				claims: claims,
				expires: expiration,
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);

			var tokenHandler = new JwtSecurityTokenHandler();

			return tokenHandler.WriteToken(token);
		}
	}
}
