using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Validators
{
    /// <summary>
    /// API Key/Secret Key must be at least minimum length
    /// and contain only valid characters (no spaces).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ApiKeyFormatAttribute : ValidationAttribute
    {
        public int MinLength { get; }

        public ApiKeyFormatAttribute(int minLength = 16)
        {
            MinLength = minLength;
            ErrorMessage = "'{0}' must be at least {1} characters long and contain no spaces.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is null or "")
                return ValidationResult.Success; 

            var str = value.ToString()!;

            if (str.Contains(' '))
            {
                return new ValidationResult(
                    $"'{context.DisplayName}' must not contain spaces.",
                    new[] { context.MemberName ?? string.Empty });
            }

            if (str.Length < MinLength)
            {
                return new ValidationResult(
                    $"'{context.DisplayName}' must be at least {MinLength} characters long.",
                    new[] { context.MemberName ?? string.Empty });
            }

            return ValidationResult.Success;
        }
    }
}
