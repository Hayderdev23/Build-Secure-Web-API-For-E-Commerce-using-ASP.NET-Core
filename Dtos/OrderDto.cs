using System.ComponentModel.DataAnnotations;

namespace BestStoreAPI.Dtos
{
    public class OrderDto
    {
        [Required]
        public string ProdectIdentifier { get; set; } = "";
        [Required , MinLength(30) , MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";
        [Required]
        public string PaymentMethod { get; set; } = "";
    }
}
