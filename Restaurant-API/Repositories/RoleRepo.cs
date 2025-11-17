using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Data.Entities;
using Restaurant_API.Repositories.Interfaces;

namespace Restaurant_API.Repositories
{
	public class RoleRepo : IRoleRepo
	{
		private readonly AppDbContext context;

		public RoleRepo(AppDbContext context)
		{
			this.context = context;
		}

		public async Task<Role?> GetByNormalizedNameAsync(string normalizedName)
		{
			return await context.Roles
								.FirstOrDefaultAsync(role => role.NormalizedName == normalizedName);
		}
	}
}
