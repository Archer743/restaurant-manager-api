using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Restaurant_API.Data;
using Restaurant_API.Repositories;
using Restaurant_API.Repositories.Interfaces;
using Restaurant_API.Services;
using Restaurant_API.Services.Interfaces;
using Restaurant_API.Settings;

namespace Restaurant_API.Tests.Integration.Fixtures
{
    /// <summary>
    /// Base for tests that exercise a service against a real database.
    ///
    /// Each test gets its own database and its own DI scope, and the clock is a FakeTimeProvider, so
    /// time-dependent rules can be asserted at a fixed instant instead of "whenever the suite happens to run".
    ///
    /// The registrations below mirror Program.cs: ConfigureServices. They are duplicated rather than reused
    /// because that method is private and calling it would also start Hangfire and demand real configuration.
    /// When a service or repo is added to Program.cs, add it here too.
    /// </summary>
    [Collection(DatabaseCollection.Name)]
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        private readonly PostgresFixture fixture;

        private ServiceProvider provider = null!;
        private AsyncServiceScope scope;

        protected IntegrationTestBase(PostgresFixture fixture)
        {
            this.fixture = fixture;
        }

        /// <summary>
        /// The controllable clock. Defaults to 2026-03-10 18:00Z, which is 20:00 in Europe/Sofia
        /// </summary>
        protected FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 3, 10, 18, 0, 0, TimeSpan.Zero));

        protected IServiceProvider Services => scope.ServiceProvider;

        protected AppDbContext Db => Services.GetRequiredService<AppDbContext>();

        protected T Resolve<T>() where T : notnull => Services.GetRequiredService<T>();

        public async Task InitializeAsync()
        {
            string connectionString = await fixture.CreateDatabaseAsync();

            var services = new ServiceCollection();

            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            // The controllable clock replaces TimeProvider.System from Program.cs
            services.AddSingleton<TimeProvider>(Clock);

            services.AddSingleton(Options.Create(new ReservationCleanup { RetentionMonths = 6 }));
            services.AddSingleton(Options.Create(new SessionSettings
            {
                RefreshTokenLength = 32,
                ServerSecretKey = "test-session-secret-key-not-a-real-one",
                SessionLifetime = TimeSpan.FromDays(30),
                RetentionDays = 30
            }));
            services.AddSingleton(Options.Create(new JwtSettings
            {
                Issuer = "restaurant-api-tests",
                Audience = "restaurant-api-tests",
                ServerSecretKey = "test-jwt-secret-key-long-enough-for-hmac-sha256",
                AccessTokenLifetime = TimeSpan.FromMinutes(15)
            }));

            // Repositories
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IRoleRepo, RoleRepo>();
            services.AddScoped<ISessionRepo, SessionRepo>();
            services.AddScoped<IRestaurantRepo, RestaurantRepo>();
            services.AddScoped<IRestaurantUserRepo, RestaurantUserRepo>();
            services.AddScoped<IShiftRepo, ShiftRepo>();
            services.AddScoped<ITableRepo, TableRepo>();
            services.AddScoped<IReservationRepo, ReservationRepo>();

            // Services
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthTokenService, AuthTokenService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<ITimeZoneService, TimeZoneService>();
            services.AddScoped<IShiftService, ShiftService>();
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<IReservationService, ReservationService>();

            provider = services.BuildServiceProvider();
            scope = provider.CreateAsyncScope();
        }

        public async Task DisposeAsync()
        {
            await scope.DisposeAsync();
            await provider.DisposeAsync();
        }
    }
}
