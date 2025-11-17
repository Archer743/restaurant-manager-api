using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MustBeUtcAttribute : ValidationAttribute
    {
        //  UTC string (e.g., ISO 8601 with a 'Z' suffix like "2025-06-25T09:30:00Z"
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Null values are typically handled by [Required] attribute
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is DateTime dateTime && dateTime.Kind == DateTimeKind.Utc)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(null);
        }
    }
}
