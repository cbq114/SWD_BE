using System.ComponentModel.DataAnnotations;

namespace Tutor.Shared.Exceptions
{
    public class ValidateDOBAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var age = DateTime.Today.Year - dateOfBirth.Year;

                if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                if (age < 16)
                {
                    return new ValidationResult(ErrorMessage);
                }

                if (age > 100)
                {
                    return new ValidationResult("Invalid date of birth.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
