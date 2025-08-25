using System.ComponentModel.DataAnnotations;


namespace United.Challenge.Api.Models
{
    public class RegisterUser : IValidatableObject
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Email
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@") || !Email.Contains("."))
                yield return new ValidationResult("Invalid email format", new[] { nameof(Email) });

            // Password length
            if (string.IsNullOrEmpty(Password) || Password.Length < 8)
                yield return new ValidationResult("Must be at least 8 characters", new[] { nameof(Password) });

            // Password has uppercase
            if (string.IsNullOrEmpty(Password) || !Password.Any(char.IsUpper))
                yield return new ValidationResult("Must contain uppercase letter", new[] { nameof(Password) });

            // ConfirmPassword matches
            if (Password != ConfirmPassword)
                yield return new ValidationResult("Passwords do not match", new[] { nameof(ConfirmPassword) });
        }
    }
}
