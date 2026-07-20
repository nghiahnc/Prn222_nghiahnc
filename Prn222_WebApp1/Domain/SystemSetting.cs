using System.ComponentModel.DataAnnotations;
using Domain.Validators;

namespace Domain
{
    public class SystemSetting
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Setting Key is required.")]
        [StringLength(100, ErrorMessage = "Setting Key cannot exceed 100 characters.")]
        [NoSpaces]
        [Display(Name = "Setting Key")]
        public string SettingKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Setting Value is required.")]
        [StringLength(2000, ErrorMessage = "Setting Value cannot exceed 2000 characters.")]
        [Display(Name = "Setting Value")]
        public string SettingValue { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
