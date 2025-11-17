namespace Restaurant_API.Attributes
{
	/// <summary>
	/// If no roles are passed, the roles Admin and Staff are authorized by default
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class AppAuthorizeAttribute : Attribute
	{
		public string[] Roles { get; init; }

		// params -> lets you pass a variable number of arguments to a method
		public AppAuthorizeAttribute(params string[] roles)
		{
			if (roles.Length == 0)
			{
				Roles = [Constants.Roles.Admin, Constants.Roles.Staff];
			} else
			{
				Roles = roles;
			}
		}
	}
}
