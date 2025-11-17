using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Data.Entities;
using Restaurant_API.Repositories.Interfaces;

namespace Restaurant_API.Repositories
{
	public class SessionRepo : ISessionRepo
	{
		private readonly AppDbContext context;

		public SessionRepo(AppDbContext context)
		{
			this.context = context;
		}

		public async Task AddAsync(Session session)
		{
			context.Sessions.Add(session);
			await context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Session session)
		{
			context.Sessions.Update(session);
			await context.SaveChangesAsync();
		}

		public async Task RemoveRangeAsync(IEnumerable<Session> sessions)
		{
			context.Sessions.RemoveRange(sessions);
			await context.SaveChangesAsync();
		}

		public async Task<Session?> GetByIdAsync(Guid id)
		{
			return await context.Sessions
								.Include(session => session.User)
								.ThenInclude(user => user.Role)
								.FirstOrDefaultAsync(session => session.Id == id);
		}

		public async Task<Session?> GetByHashedTokenAsync(string refreshTokenHash)
		{
			return await context.Sessions
								.Include(session => session.User)
								.ThenInclude(user => user.Role)
								.FirstOrDefaultAsync(session => session.RefreshTokenHash == refreshTokenHash);
		}

        public async Task<bool> HashExistsAsync(string refreshTokenHash)
        {
			return await context.Sessions
								.AnyAsync(session => session.RefreshTokenHash == refreshTokenHash);
        }

        public async Task<List<Session>> GetOldInactiveSessionsAsync(uint thresholdDays)
		{
			var thresholdDate = DateTime.UtcNow.AddDays(-thresholdDays);

			return await context.Sessions
								.Where(session =>
									(session.ExpiresAt < DateTime.UtcNow || session.IsRevoked)
									&& session.ExpiresAt < thresholdDate)
								.ToListAsync();
		}
    }
}
