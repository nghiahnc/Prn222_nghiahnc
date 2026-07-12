using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class PaymentGatewayConfig
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Provider Name is required.")]
        [StringLength(100, ErrorMessage = "Provider Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "API Key is required.")]
        [StringLength(255, ErrorMessage = "API Key cannot exceed 255 characters.")]
        public string ApiKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Secret Key is required.")]
        [StringLength(255, ErrorMessage = "Secret Key cannot exceed 255 characters.")]
        public string SecretKey { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
