using Microsoft.EntityFrameworkCore;
using Npgsql;
using Restaurant_API.Data;
using Restaurant_API.Data.Entities;
using Testcontainers.PostgreSql;

namespace Restaurant_API.Tests.Integration.Fixtures
{
    /// <summary>
    /// One PostgreSQL container for the whole test run.
    ///
    /// The container is migrated once into a template database, and every test class then gets its own
    /// database cloned from that template. Cloning costs milliseconds, while re-running the 24 migrations
    /// per class would not, and a database per class gives full isolation without relying on rollback
    /// (which breaks as soon as a service opens its own transaction).
    ///
    /// Real PostgreSQL rather than the in-memory provider on purpose: shift and reservation logic depends
    /// on timestamptz semantics, on TimeOnly comparisons and on the unique indexes, none of which the
    /// in-memory provider reproduces.
    /// </summary>
    public sealed class PostgresFixture : IAsyncLifetime
    {
        private const string TemplateDatabase = "restaurant_template";

        private readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase(TemplateDatabase)
            .Build();

        public async Task InitializeAsync()
        {
            await container.StartAsync();

            await using (AppDbContext db = CreateContext(container.GetConnectionString()))
            {
                await db.Database.MigrateAsync();
                await SeedRequiredRolesAsync(db);
            }

            // PostgreSQL refuses CREATE DATABASE ... TEMPLATE while any session is still connected
            // to the template, and a pooled connection stays open after the context is disposed.
            NpgsqlConnection.ClearAllPools();
        }

        public Task DisposeAsync() => container.DisposeAsync().AsTask();

        /// <summary>Clones the migrated template into a fresh database and returns its connection string.</summary>
        public async Task<string> CreateDatabaseAsync()
        {
            string databaseName = "test_" + Guid.NewGuid().ToString("N");

            await using var connection = new NpgsqlConnection(MaintenanceConnectionString());
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(
                $"""CREATE DATABASE "{databaseName}" TEMPLATE "{TemplateDatabase}" """, connection);

            await command.ExecuteNonQueryAsync();

            return ConnectionStringFor(databaseName);
        }

        public static AppDbContext CreateContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new AppDbContext(options);
        }

        /// <summary>
        /// Mirrors HostExtensions.SeedRequiredDataAsync, but only the roles. The restaurants it also seeds
        /// are deliberately left out: every test seeds the restaurant it needs via RestaurantScenario.
        /// </summary>
        private static async Task SeedRequiredRolesAsync(AppDbContext db)
        {
            if (await db.Roles.AnyAsync()) return;

            db.Roles.AddRange(
                new Role { Name = "Staff", NormalizedName = "STAFF" },
                new Role { Name = "Admin", NormalizedName = "ADMIN" }
            );

            await db.SaveChangesAsync();
        }

        private string ConnectionStringFor(string databaseName)
        {
            return new NpgsqlConnectionStringBuilder(container.GetConnectionString())
            {
                Database = databaseName
            }.ConnectionString;
        }

        /// <summary>CREATE DATABASE must run from a connection to a different database.</summary>
        private string MaintenanceConnectionString()
        {
            return new NpgsqlConnectionStringBuilder(container.GetConnectionString())
            {
                Database = "postgres",
                Pooling = false
            }.ConnectionString;
        }
    }
}
