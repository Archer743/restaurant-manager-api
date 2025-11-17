using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Restaurant_API.Data.Entities;
using Restaurant_API.Data.Entities.Enums;

namespace Restaurant_API.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		
		public DbSet<User> Users { get; set; }

		public DbSet<Role> Roles { get; set; }

		public DbSet<Session> Sessions { get; set; }

		public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<RestaurantUser> RestaurantUsers { get; set; }

        public DbSet<Shift> Shifts { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

		public DbSet<Table> Tables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Makes User.Username Unique
			modelBuilder.Entity<User>()
						.HasIndex(u => u.Username)
						.IsUnique();

			// Makes Role.NormalizedName Unique
			modelBuilder.Entity<Role>()
						.HasIndex(r => r.NormalizedName)
						.IsUnique();

            // Makes Session.RefreshTokenHash Unique
            modelBuilder.Entity<Session>()
                        .HasIndex(s => s.RefreshTokenHash)
                        .IsUnique();

            // Makes composite primary key (RestaurantId + UserId)
            modelBuilder.Entity<RestaurantUser>()
						.HasKey(ru => new { ru.RestaurantId, ru.UserId });

            // Makes Table.TableNumber Unique for a restaurant
            modelBuilder.Entity<Table>()
                        .HasIndex(t => new { t.RestaurantId, t.TableNumber })
						.IsUnique();

            // Configures Entity Framework Core to store the ReservationState enum as its string representation,
            // preventing data corruption if the integer values of the enum members are changed in the future
            /*modelBuilder.Entity<Reservation>()
                .Property(r => r.State)
                .HasConversion<string>();*/

            // Makes Reservation.Tag Unique for a restaurant
            modelBuilder.Entity<Reservation>()
						.HasIndex(r => new { r.RestaurantId, r.Tag })
						.IsUnique();

            // Many-to-many relation between Reservation and Table entities
            modelBuilder.Entity<Reservation>()
						.HasMany(r => r.Tables)
						.WithMany(t => t.Reservations)
						.UsingEntity(j => j.ToTable("ReservationTables"));
        }
	}
}
