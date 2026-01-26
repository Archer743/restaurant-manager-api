using Restaurant_API.Data.Entities;

namespace Restaurant_API.Repositories.Interfaces
{
	public interface ISessionRepo
	{
		Task AddAsync(Session session);

		Task UpdateAsync(Session session);

		Task<int> RemoveInactiveSessionsBeforeAsync(DateTime cutoffUtc);

		Task<Session?> GetByIdAsync(Guid id);

		Task<Session?> GetByHashedTokenAsync(string refreshTokenHash);

        Task<bool> HashExistsAsync(string refreshTokenHash);
	}
}
