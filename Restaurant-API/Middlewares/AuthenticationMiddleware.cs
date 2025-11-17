using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Restaurant_API.Services.Interfaces;
using Restaurant_API.Settings;
using System.Text;
using Restaurant_API.Exceptions;
using Restaurant_API.Constants;
using System.IdentityModel.Tokens.Jwt;
using Restaurant_API.Data.Entities;

namespace Restaurant_API.Middlewares
{
	public class AuthenticationMiddleware : BaseAuthMiddleware
	{
		private readonly TokenValidationParameters validParams;

		public AuthenticationMiddleware(RequestDelegate next, IOptions<JwtSettings> settings) : base(next)
		{
			JwtSettings jwtSettings = settings.Value;
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.ServerSecretKey));

			validParams = new TokenValidationParameters
			{
				ValidIssuer = jwtSettings.Issuer,
				ValidAudience = jwtSettings.Audience,
				IssuerSigningKey = key,

				// Presence requirements
				RequireSignedTokens = true,
				RequireExpirationTime = true,
				RequireAudience = true,

				// Value validations
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateIssuerSigningKey = true,
				ValidateLifetime = true,

				// A time-tolerant feature that is applied during token validation to compensate for possible
				// clock differences between different servers

				// Single server				=> 0s
				// Cloud app					=> 30s - 1min
				// Microservice environment		=> 2-5 min

				ClockSkew = TimeSpan.FromMinutes(2),
			};
		}

		public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
		{
			var endpointAuth = GetEndpointAuthorization(context);

			// Validate if the endpoint requires authentication. If it doesn't => continue with the next middleware
			if (endpointAuth == null)
			{
				await next(context);
				return;
			}

			var handler = new JwtSecurityTokenHandler();

			// All exceptions in this block result in UnauthorizedException for security reasons
			try
			{
				// Attempt to extract the access token from the Authorization header
				string accessToken = GetAccessToken(context);

				// Validate the access token and extract the associated claims
				var claimsPrincipal = handler.ValidateToken(accessToken, validParams, out _);

				// Retrieve userId and sessionId claims
				string userIdClaim = claimsPrincipal.FindFirst("userId")!.Value;
				string sessionIdClaim = claimsPrincipal.FindFirst("sessionId")!.Value;

				// Parse the ids from the claims
				Guid userId = Guid.Parse(userIdClaim);
				Guid sessionId = Guid.Parse(sessionIdClaim);

				// Retrive the session and validate if it's active and belongs to the current user
				Session? session = await sessionService.GetSessionByIdAsync(sessionId);

				if (session == null || session.UserId != userId || sessionService.IsSessionInactive(session))
				{
					throw new Exception();
				}

				// Store the session for downstream use. It includes information about the user and their role
				context.Items["Session"] = session;
			}
			catch
			{
				throw new UnauthorizedException(ErrorCodes.Auth.InvalidAuthorization);
			}

			await next(context);
		}

		private string GetAccessToken(HttpContext context)
		{
			/*
			 * The request must include an 'Authorization' header following the standard format:
			 * Authorization: Bearer <access_token>
			 *
			 * This ensures the server can extract and validate the JWT access token.
			 */

			// Prevents issues in rare cases where multiple Authorization headers are present
			string? authHeader = context.Request.Headers.Authorization.FirstOrDefault();

			/*
			 * IsNullOrEmpty -> handles only null and ""
			 * IsNullOrWhiteSpace -> handles null, "" and white space chars
			 * StringComparison.OrdinalIgnoreCase -> Ignores casing (Bearer, bearer, BEARER, etc. all match).
			 */

			if (string.IsNullOrWhiteSpace(authHeader) ||
				!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
			{
				throw new Exception();
			}

			return authHeader["Bearer ".Length..].Trim();
		}
	}

	public static class AuthenticationMiddlewareExtensions
	{
		public static IApplicationBuilder UseAppAuthentication(this IApplicationBuilder app)
		{
			return app.UseMiddleware<AuthenticationMiddleware>();
		}
	}
}
