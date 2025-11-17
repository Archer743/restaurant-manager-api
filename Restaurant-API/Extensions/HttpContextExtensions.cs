using Restaurant_API.Data.Entities;

namespace Restaurant_API.Extensions
{
	public static class HttpContextExtensions
	{
		public static Session GetCurrSession(this HttpContext ctx)
		{
			if (ctx.Items.TryGetValue("Session", out object? sessionObject) && sessionObject is Session session)
			{
				return session;
			}

			// The authentication middleware guarantees the existence of the session
			throw new InvalidOperationException("Session entity is missing from HttpContext.Items. Authentication check failed or was bypassed.");
		}

		public static User GetCurrUser(this HttpContext ctx)
		{
			Session currSession = ctx.GetCurrSession();
			return currSession.User;
		}

		public static Role GetCurrUserRole(this HttpContext ctx)
		{
			Session currSession = ctx.GetCurrSession();
			return currSession.User.Role;
		}
	}
}
