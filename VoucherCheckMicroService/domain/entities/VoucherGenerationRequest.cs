using System.ComponentModel.DataAnnotations;

namespace VoucherCheckService.domain.entities
{
    public class VoucherGenerationRequest
    {
        [Required(ErrorMessage = "registration is required")]
        [MinLength(1, ErrorMessage = "registration is required")]
        public string registration { get; set; }
        [Required(ErrorMessage = "productSku is required")]
        [MinLength(1, ErrorMessage = "productSku is required")]
        public string productSku { get; set; }
    }
}