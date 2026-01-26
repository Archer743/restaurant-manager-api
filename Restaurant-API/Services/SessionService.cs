using Microsoft.Extensions.Options;
using Restaurant_API.Data.Entities;
using Restaurant_API.Repositories.Interfaces;
using Restaurant_API.Services.Interfaces;
using Restaurant_API.Settings;
using Restaurant_API.Exceptions;
using System.Security.Cryptography;
using System.Text;
using Restaurant_API.Constants;
using Restaurant_API.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Restaurant_API.Extensions;

namespace Restaurant_API.Services
{
    public class SessionService : ISessionService
	{
        private enum SessionOperationType { Create, Refresh }

        private readonly SessionSettings sessionSettings;
		private readonly IAuthTokenService authTokenService;
		private readonly ISessionRepo sessionRepo;

        private readonly ILogger<SessionService> logger;

		public SessionService(
            IOptions<SessionSettings> settings,
            IAuthTokenService authTokenService,
            ISessionRepo sessionRepo,
			ILogger<SessionService> logger)
		{
			sessionSettings = settings.Value;
			this.authTokenService = authTokenService;
			this.sessionRepo = sessionRepo;
            this.logger = logger;
		}

        public async Task<Session?> GetSessionByIdAsync(Guid sessionId)
        {
            return await sessionRepo.GetByIdAsync(sessionId);
        }

		public async Task<UserAuthModel> CreateNewSessionAsync(Guid userId)
		{
            // Generate access and unique refresh tokens
            Guid sessionId = Guid.NewGuid();
            UserAuthModel tokens = await GenerateUniqueAuthTokensAsync(userId, sessionId, SessionOperationType.Create);

            var session = new Session
            {
                Id = sessionId,
                UserId = userId,
                RefreshTokenHash = GetHashedRefreshToken(tokens.RefreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(sessionSettings.SessionLifetime),
                IsRevoked = false
            };

			try
			{
                // Save new session
                await sessionRepo.AddAsync(session);
                return tokens;
            }
            catch (DbUpdateException)
            {
                // Race Condition
                throw new ServiceUnavailableException(ErrorCodes.Session.CreationFailed);
            }
        }

		public async Task<UserAuthModel> RefreshSessionAsync(string refreshToken)
		{
			string hashedToken = GetHashedRefreshToken(refreshToken);
			Session? session = await sessionRepo.GetByHashedTokenAsync(hashedToken);

			if (session == null || IsSessionInactive(session))
			{
				throw new UnauthorizedException(ErrorCodes.Session.InvalidSession);
			}

            // Generate access and unique refresh tokens
            UserAuthModel newTokens = await GenerateUniqueAuthTokensAsync(session.UserId, session.Id, SessionOperationType.Refresh);

			try
			{
                // Update session's refresh token & extend the session
                session.RefreshTokenHash = GetHashedRefreshToken(newTokens.RefreshToken);
                session.ExpiresAt = DateTime.UtcNow.Add(sessionSettings.SessionLifetime);
                await sessionRepo.UpdateAsync(session);

                return newTokens;
            }
            catch (DbUpdateException)
            {
                // Race Condition
                throw new ServiceUnavailableException(ErrorCodes.Session.UpdateFailed);
            }
        }

		public async Task EndCurrSessionAsync(HttpContext ctx)
        {
			Session currSession = ctx.GetCurrSession();
            currSession.IsRevoked = true;
			await sessionRepo.UpdateAsync(currSession);
		}

		public async Task CleanupInactiveSessionsAsync()
		{
            DateTime cutoffUtc = DateTime.UtcNow.AddDays(-sessionSettings.RetentionDays);

            int removed = await sessionRepo.RemoveInactiveSessionsBeforeAsync(cutoffUtc);

			if (removed > 0) logger.LogInformation("Removed {RemovedCount} inactive sessions!", removed);
		}

        public bool IsSessionInactive(Session session)
		{
			// Inactive = expired or revoked
			return session.ExpiresAt < DateTime.UtcNow || session.IsRevoked;
		}

		private async Task<UserAuthModel> GenerateUniqueAuthTokensAsync(Guid userId, Guid sessionId, SessionOperationType operationType)
		{
            const int MaxHashRetries = 3;

            // Generate tokens
            UserAuthModel tokens = authTokenService.GenerateAuthTokens(userId, sessionId);

            string hashedToken = GetHashedRefreshToken(tokens.RefreshToken);
            bool isHashUnique = false;

            // Ensure that the refresh token's hash is unique
            for (int attempt = 0; attempt < MaxHashRetries; attempt++)
            {
                if (!await sessionRepo.HashExistsAsync(hashedToken))
                {
                    isHashUnique = true;
					break;
                }
                else if (attempt == MaxHashRetries - 1)
                {
                    break;
                }

                tokens.RefreshToken = authTokenService.GenerateRefreshToken();
                hashedToken = GetHashedRefreshToken(tokens.RefreshToken);
            }

            if (!isHashUnique)
            {
                var errorCode = operationType == SessionOperationType.Create
                    ? ErrorCodes.Session.CreationFailed
                    : ErrorCodes.Session.UpdateFailed;

                throw new ServiceUnavailableException(errorCode);
            }

			return tokens;
        }

        private string GetHashedRefreshToken(string refreshToken)
		{
			using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(sessionSettings.ServerSecretKey));
			byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
			return Convert.ToBase64String(hashBytes);
		}
	}
}
