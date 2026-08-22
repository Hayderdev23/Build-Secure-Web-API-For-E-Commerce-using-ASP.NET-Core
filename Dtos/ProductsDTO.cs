using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreAPI.Dtos
{
    public class ProductsDTO
    {
        [Required , MaxLength(100)]
        public string Name { get; set; } = "";
        [Required , MaxLength(100)]
        public string Brand { get; set; } = "";
        [MaxLength(100)]
        public string Category { get; set; } = "";

        [Required]
        public decimal price { get; set; }
        public string? Description { get; set; } = "";
        [Required]
        public IFormFile? ImageFileName { get; set; }
    }
}
