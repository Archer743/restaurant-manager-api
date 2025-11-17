using Restaurant_API.Services.Interfaces;

namespace Restaurant_API.Services
{
	public class PasswordHasherService : IPasswordHasherService
	{
		private readonly int workFactor = 13;
		private readonly char bcryptMinorRevision = 'b';
		private readonly bool enhancedEntropy = false;

		public string HashPassword(string password)
		{
			string salt = BCrypt.Net.BCrypt.GenerateSalt(workFactor, bcryptMinorRevision);
			return BCrypt.Net.BCrypt.HashPassword(password, salt, enhancedEntropy);
		}

		public bool VerifyPassword(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword, enhancedEntropy);
		}
	}
}
