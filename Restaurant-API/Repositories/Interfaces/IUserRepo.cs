using Restaurant_API.Data.Entities;

namespace Restaurant_API.Repositories.Interfaces
{
	public interface IUserRepo
	{
		Task AddAsync(User user);

		Task<User?> GetByUsernameAsync(string username);

		Task<User?> GetByIdAsync(Guid id);
	}
}
