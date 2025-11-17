using Restaurant_API.Data.Models.Auth;

namespace Restaurant_API.Services.Interfaces
{
    public interface IAuthTokenService
	{
		UserAuthModel GenerateAuthTokens(Guid userId, Guid sessionId);

        string GenerateRefreshToken();
    }
}
