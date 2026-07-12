using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SystemSetting
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Setting Key is required.")]
        [StringLength(100, ErrorMessage = "Setting Key cannot exceed 100 characters.")]
        public string SettingKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Setting Value is required.")]
        public string SettingValue { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
