using Restaurant_API.Data.Models.Auth;
using Restaurant_API.Data.Entities;

namespace Restaurant_API.Services.Interfaces
{
    public interface ISessionService
	{
		Task<Session?> GetSessionByIdAsync(Guid sessionId);

		Task<UserAuthModel> CreateNewSessionAsync(Guid userId);

		Task<UserAuthModel> RefreshSessionAsync(string refreshToken);

		Task EndCurrSessionAsync(HttpContext ctx);

		Task RemoveOldInactiveSessionsAsync();

		bool IsSessionInactive(Session session);
	}
}
