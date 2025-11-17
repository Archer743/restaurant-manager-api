using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Restaurant_API.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
	public class PasswordAttribute : ValidationAttribute
	{
		/* Valid password must:
		 * Be 8 to 16 characters long
		 * Contain at least one lowercase letter (a-z)
		 * Contain at least one uppercase letter (A-Z)
		 * Contain at least one digit (0-9)
		 * Contain at least one special character from this set: !@#$%^&*()_+
		 */
		private static readonly string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d!@#$%^&*()_+]{8,16}$";

		private readonly Regex re = new Regex(pattern);

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			string? password = value as string;

			if (password == null || password == "" || !re.IsMatch(password))
				return new ValidationResult(null);

			return ValidationResult.Success;
		}
	}
}
