using Restaurant_API.Data.Entities;

namespace Restaurant_API.Repositories.Interfaces
{
	public interface ISessionRepo
	{
		Task AddAsync(Session session);

		Task UpdateAsync(Session session);

		Task RemoveRangeAsync(IEnumerable<Session> sessions);

		Task<Session?> GetByIdAsync(Guid id);

		Task<Session?> GetByHashedTokenAsync(string refreshTokenHash);

        Task<bool> HashExistsAsync(string refreshTokenHash);

        Task<List<Session>> GetOldInactiveSessionsAsync(uint thresholdDays);
    }
}
