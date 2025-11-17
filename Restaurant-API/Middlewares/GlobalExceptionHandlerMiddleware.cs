using Restaurant_API.Exceptions;
using System.Net;
using System.Text.Json;

namespace Restaurant_API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
		private readonly RequestDelegate next;

		public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (HttpException e)
            {
                await WriteErrorResponseAsync(context, e.StatusCode, e.ErrorCode);
            }
            catch (Exception e)
            {
			    // TODO: Add Logger
			    Console.WriteLine($"[LOGGER | Error] : {e.Message}");
                await WriteErrorResponseAsync(context, HttpStatusCode.InternalServerError);
            }
        }

		private async Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, string? errorCode = null)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)statusCode;

			var response = JsonSerializer.Serialize(new { errorCode });

			await context.Response.WriteAsync(response);
		}
	}

    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
