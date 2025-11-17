using Restaurant_API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Data.Models.User
{
	public class UserRegisterModel
	{
		[StringLength(40, MinimumLength = 3)]
		public string Username { get; init; }


		[StringLength(50, MinimumLength = 1)]
		public string FirstName { get; init; }


		[StringLength(50, MinimumLength = 1)]
		public string LastName { get; init; }


		// HTTPS encrypts the entire request so sending raw password is not a problem
		// Hashing happens in the backend
		[Password]
		public string Password { get; init; }
	}
}
