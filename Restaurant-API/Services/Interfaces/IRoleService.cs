namespace Restaurant_API.Services.Interfaces
{
	public interface IRoleService
	{
		Task<Guid> GetRoleIdByNormalizedNameAsync(string normalizedName);

		bool HasCurrUserRole(HttpContext ctx, string role);

		bool HasCurrUserAnyRole(HttpContext ctx, IEnumerable<string> roles);
    }
}
