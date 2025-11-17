using Restaurant_API.Data.Entities;

namespace Restaurant_API.Repositories.Interfaces
{
	public interface IRoleRepo
	{
		Task<Role?> GetByNormalizedNameAsync(string normalizedName);
	}
}
