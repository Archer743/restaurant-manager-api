using Restaurant_API.Attributes;

namespace Restaurant_API.Middlewares
{
	public class BaseAuthMiddleware
	{
		protected readonly RequestDelegate next;

		public BaseAuthMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		protected AppAuthorizeAttribute? GetEndpointAuthorization(HttpContext context)
		{
			var currEnpoint = context.GetEndpoint();
			return currEnpoint?.Metadata?.GetMetadata<AppAuthorizeAttribute>();
		}
	}
}
