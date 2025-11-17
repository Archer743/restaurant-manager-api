using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Restaurant_API.Attributes
{
    /*
     * This custom attribute solves the ambiguity of null and empty strings in PATCH requests.
     * [EmailAddress] treats an empty string as an invalid value, which prevents the server from using it to explicitly clear a field.
     * The custom attribute allows you to differentiate between:
     * - null: The field is not being changed;
     * - "" (empty string): The field should be cleared or deleted.
     */
    public class OptionalEmailAttribute : ValidationAttribute
    {
        private static readonly Regex _emailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // 1. Check for null or empty string. This is the "optional" part.
            if (value == null || (value is string s && string.IsNullOrEmpty(s)))
            {
                return ValidationResult.Success;
            }

            // 2. If the value is a string, perform regex validation.
            if (value is string email && _emailRegex.IsMatch(email))
            {
                return ValidationResult.Success;
            }

            // 3. If validation fails for any other case, return an error
            return new ValidationResult(null);
        }
    }
}
