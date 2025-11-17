using Restaurant_API.Data.Entities;
using Restaurant_API.Extensions;
using Restaurant_API.Repositories.Interfaces;
using Restaurant_API.Services.Interfaces;

namespace Restaurant_API.Services
{
	public class RoleService : IRoleService
	{
		private readonly IRoleRepo roleRepo;

		public RoleService(IRoleRepo roleRepo)
		{
			this.roleRepo = roleRepo;
		}

		public async Task<Guid> GetRoleIdByNormalizedNameAsync(string normalizedName)
		{
			Role? role = await roleRepo.GetByNormalizedNameAsync(normalizedName);

			if (role == null)
			{
				throw new Exception($"A role with normalized name '{normalizedName}' was not found in the database.");
			}

			return role.Id;
		}

		public bool HasCurrUserRole(HttpContext ctx, string role)
		{
			string currUserRoleName = ctx.GetCurrUserRole().NormalizedName;
			return string.Equals(currUserRoleName, role, StringComparison.OrdinalIgnoreCase);
		}

		public bool HasCurrUserAnyRole(HttpContext ctx, IEnumerable<string> roles)
		{
			string currUserRoleName = ctx.GetCurrUserRole().NormalizedName;
			return roles.Any(role => string.Equals(currUserRoleName, role, StringComparison.OrdinalIgnoreCase));
		}
    }
}
