using Microsoft.AspNetCore.Http;
using Restaurant_API.Data.Entities;

namespace Restaurant_API.Tests.Common
{
    /// <summary>
    /// Builds the HttpContext that services expect. Services resolve the caller through
    /// ctx.GetCurrUser()/GetCurrUserRole(), which read the Session that AuthenticationMiddleware
    /// stashes in Items["Session"]. Putting it there directly lets a service be tested without HTTP.
    /// </summary>
    public static class FakeCaller
    {
        public static HttpContext For(User user)
        {
            var ctx = new DefaultHttpContext();

            ctx.Items["Session"] = new Session
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                RefreshTokenHash = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UnixEpoch,
                ExpiresAt = DateTime.MaxValue.ToUniversalTime(),
                IsRevoked = false
            };

            return ctx;
        }
    }
}
