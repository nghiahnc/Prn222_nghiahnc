using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Domain.Validators
{   
    /// <summary>
    /// Setting Key must contain only letters, numbers, underscores, hyphens, or dots (no spaces).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NoSpacesAttribute : ValidationAttribute
    {
        private static readonly Regex _valid = new Regex(@"^[A-Za-z0-9_\-\.]+$", RegexOptions.Compiled);

        public NoSpacesAttribute()
        {
            ErrorMessage = "'{0}' must contain only letters, numbers, underscores, hyphens, or dots (no spaces).";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is null or "")
                return ValidationResult.Success; 

            var str = value.ToString()!;

            if (!_valid.IsMatch(str))
            {
                return new ValidationResult(
                    FormatErrorMessage(context.DisplayName),
                    new[] { context.MemberName ?? string.Empty });
            }

            return ValidationResult.Success;
        }
    }
}
