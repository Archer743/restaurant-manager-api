using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Hangfire;
using Hangfire.PostgreSql;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Sockets;
using Polly;
using Npgsql;
using Restaurant_API.Constants;
using Restaurant_API.Data;
using Restaurant_API.Middlewares;
using Restaurant_API.Repositories;
using Restaurant_API.Repositories.Interfaces;
using Restaurant_API.Services;
using Restaurant_API.Services.Interfaces;
using Restaurant_API.Settings;
using Restaurant_API.Extensions;

namespace Restaurant_API
{
	public class Program
	{
		// Retrieves server configurations and validates them
		private static void ConfigureSettings(WebApplicationBuilder builder)
		{
			builder.Services.AddOptions<ConnectionStrings>().BindConfiguration("ConnectionStrings")
							.ValidateDataAnnotations().ValidateOnStart();

			builder.Services.AddOptions<JwtSettings>().BindConfiguration("JwtSettings")
							.ValidateDataAnnotations().ValidateOnStart();

			builder.Services.AddOptions<SessionSettings>().BindConfiguration("SessionSettings")
							.ValidateDataAnnotations().ValidateOnStart();

			builder.Services.AddOptions<ReservationCleanup>().BindConfiguration("ReservationCleanup")
							.ValidateDataAnnotations().ValidateOnStart();
        }

		private static void ConfigureServices(WebApplicationBuilder builder)
		{
            // Support for controllers, model binding, attribute routing, etc.
            // By default, enums are serialized as integers. This config serializes them as strings instead (camelCase),  
            // which is safer because changing the enum order won’t affect the JSON output.
            builder.Services.AddControllers()
							.AddJsonOptions(options =>
							{
								options.JsonSerializerOptions.Converters.Add(
									new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
								);
							});

			builder.Services.AddEndpointsApiExplorer();  // Swagger can find and display all API endpoints
			builder.Services.AddSwaggerGen();  // Swagger UI to test the API in the browser


			// ASP.NET Core automatically validates the input model before it reaches the action method by default
			// Overrides the default automatic 400 Bad Request response behavior
			builder.Services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = context =>
				{
					// TODO: Specify what is invalid in each model field [FUTURE]
					var errorResponse = new
					{
						errorCode = ErrorCodes.Data.InvalidModel
					};

					return new BadRequestObjectResult(errorResponse);
				};
			});


			// Cors policies
			builder.Services.AddCors(options =>
			{
				// All browser requests from external websites will receive a CORS error.
				// The mobile app will not have any problems — it uses native http client (it's not a browser)
				// so it will bypass the CORS.
				options.AddPolicy("StrictPolicy", policy =>
				{
					policy.DisallowCredentials();  // Prohibits sending "credentials" (cookies, auth header)
				});

				// !!! ONLY USED IN DEVELOPMENT !!!
				// Allows receiving requests from other domains.
				options.AddPolicy("DevPolicy", policy =>
				{
					policy
						.AllowAnyOrigin()
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});


			// Database Setup
			var connectionStrings = builder.Services.BuildServiceProvider()
													.GetService<IOptions<ConnectionStrings>>()!.Value;

			builder.Services.AddDbContext<AppDbContext>(c => c.UseNpgsql(connectionStrings.PostgresConnection));


			// Hangfire Setup
			builder.Services.AddHangfire(config => config
					.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)  // Use compatibility rules from Hangfire 1.8.0 (and later)
					.UseSimpleAssemblyNameTypeSerializer()
					.UseRecommendedSerializerSettings()
					.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionStrings.PostgresConnection))
			);

			builder.Services.AddHangfireServer();  // Adds a background worker that processes jobs


			// Clock abstraction. Services take a dependency on this instead of DateTime.UtcNow
			// so time-dependent rules (shift windows, token/session expiry) are testable at a fixed instant.
			builder.Services.AddSingleton(TimeProvider.System);


			/* builder.Services
			 * .AddSingleton	=> One instance is created and shared across the entire app lifetime.
			 * .AddScoped		=> One instance per HTTP request.
			 * .AddTransient	=> New instance every time it’s requested.
			 */

			// Repositories Setup
			builder.Services.AddScoped<IUserRepo, UserRepo>();
			builder.Services.AddScoped<IRoleRepo, RoleRepo>();
			builder.Services.AddScoped<ISessionRepo, SessionRepo>();

            builder.Services.AddScoped<IRestaurantRepo, RestaurantRepo>();
			builder.Services.AddScoped<IRestaurantUserRepo, RestaurantUserRepo>();
            builder.Services.AddScoped<IShiftRepo, ShiftRepo>();
            builder.Services.AddScoped<ITableRepo, TableRepo>();

            builder.Services.AddScoped<IReservationRepo, ReservationRepo>();

            // Services Setup
			builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddScoped<IRoleService, RoleService>();
			builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
			builder.Services.AddScoped<ISessionService, SessionService>();

			builder.Services.AddScoped<IRestaurantService, RestaurantService>();
			builder.Services.AddScoped<ITimeZoneService, TimeZoneService>();
			builder.Services.AddScoped<IShiftService, ShiftService>();
            builder.Services.AddScoped<ITableService, TableService>();
			builder.Services.AddScoped<IReservationService, ReservationService>();
        }

		// Hangfire recurring jobs setup
		private static void ConfigureRecurringJobs(WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

			recurringJobManager.AddOrUpdate<ISessionService>(
				"cleanup-inactive-sessions-job",
				service => service.CleanupInactiveSessionsAsync(),
				Cron.Daily()
			);

			recurringJobManager.AddOrUpdate<IReservationService>(
				"cleanup-reservations-past-retention-job",
				service => service.CleanupReservationsPastRetentionAsync(),
				Cron.Daily()
			);
		}

		private static void ConfigureApp(WebApplication app)
		{
			// Allow Nginx to forward HTTPS and client IP info
			var forwardedHeaderOptions = new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor |
								ForwardedHeaders.XForwardedProto |
        						ForwardedHeaders.XForwardedHost
			};

			// Allow internal Docker network
			forwardedHeaderOptions.KnownNetworks.Clear();
			forwardedHeaderOptions.KnownProxies.Clear();

			app.UseForwardedHeaders(forwardedHeaderOptions);


			// Middlewares setup
			if (app.Environment.IsDevelopment())
			{
				/// Dev environment
				app.UseSwagger();
				app.UseSwaggerUI();
				app.UseHangfireDashboard("/hangfire");

				app.UseCors("DevPolicy");
			} else if (app.Environment.IsProduction() || app.Environment.IsStaging())
			{
				/// Production environment
				app.UseCors("StrictPolicy");
			}

			// Ordering is critical: GlobalExceptionHandler -> Authentication -> Authorization
			app.UseGlobalExceptionHandler();
			app.UseAppAuthentication();
			app.UseAppAuthorization();


			// Routes requests to controller actions
			app.MapControllers();


			ConfigureRecurringJobs(app);
		}

		private static async Task WaitForDatabaseAsync(WebApplication app)
		{
			var connectionStrings = app.Services.GetRequiredService<IOptions<ConnectionStrings>>().Value;

			var retryPolicy = Policy
				.Handle<NpgsqlException>()
				.Or<SocketException>()
				.WaitAndRetryAsync(
					retryCount: 10,
					sleepDurationProvider: attempt => TimeSpan.FromSeconds(attempt * 2), // 2s, 4s, 6s...
					onRetry: (exception, timeSpan, attempt, _) =>
					{
						app.Logger.LogWarning(
							"Database not ready (attempt {Attempt}/10). Retrying in {Delay}s. Error: {Message}",
							attempt, timeSpan.TotalSeconds, exception.Message
						);
					}
				);

			await retryPolicy.ExecuteAsync(async () =>
			{
				await using var conn = new NpgsqlConnection(connectionStrings.PostgresConnection);
				await conn.OpenAsync();
				await using var cmd = new NpgsqlCommand("SELECT 1", conn);
				await cmd.ExecuteScalarAsync();
			});

			app.Logger.LogInformation("Database is ready.");
		}

		public static async Task Main(string[] args)
		{
			// Create an app builder
			var builder = WebApplication.CreateBuilder(args);
			ConfigureSettings(builder);
			ConfigureServices(builder);

			// Create an app based on the builder
			var app = builder.Build();
			app.Logger.LogInformation(
				"==================================== Server running in environment: {env}",
				app.Environment.EnvironmentName
			);

			// If the app environment is `Migration` the app will apply db migrations, seed the db
			// and exit gracefully without running the server.
			if (app.Environment.EnvironmentName.Equals("Migration", StringComparison.OrdinalIgnoreCase))
			{
				await app.MigrateAndSeedDbAsync();
				Environment.Exit(0);
				return;
			}

			await WaitForDatabaseAsync(app);

			ConfigureApp(app);
			await app.RunAsync();
		}
	}
}
