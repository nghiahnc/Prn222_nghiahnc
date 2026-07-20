using System.ComponentModel.DataAnnotations;
using Domain.Validators;

namespace Domain
{
    public class PaymentGatewayConfig
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Provider Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Provider Name must be between 2 and 100 characters.")]
        [Display(Name = "Provider Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "API Key is required.")]
        [StringLength(255, ErrorMessage = "API Key cannot exceed 255 characters.")]
        [ApiKeyFormat(16)]
        [Display(Name = "API Key")]
        public string ApiKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Secret Key is required.")]
        [StringLength(255, ErrorMessage = "Secret Key cannot exceed 255 characters.")]
        [ApiKeyFormat(16)] 
        [Display(Name = "Secret Key")]
        public string SecretKey { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
