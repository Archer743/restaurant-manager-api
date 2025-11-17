using Restaurant_API.Exceptions;
using Restaurant_API.Services.Interfaces;

namespace Restaurant_API.Middlewares
{
	public class AuthorizationMiddleware : BaseAuthMiddleware
	{
		public AuthorizationMiddleware(RequestDelegate next) : base(next) { }

		public async Task InvokeAsync(HttpContext context, IRoleService roleService)
		{
			var endpointAuth = GetEndpointAuthorization(context);

			// Validate if the endpoint requires authorization
			if (endpointAuth == null || endpointAuth.Roles.Length == 0)
			{
				await next(context);
				return;
			}

			if (!roleService.HasCurrUserAnyRole(context, endpointAuth.Roles))
			{
				throw new ForbiddenException();
			}

			await next(context);
		}
	}

	public static class AuthorizationMiddlewareExtensions
	{
		public static IApplicationBuilder UseAppAuthorization(this IApplicationBuilder app)
		{
			return app.UseMiddleware<AuthorizationMiddleware>();
		}
	}
}
