using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BestStoreAPI.Dtos
{
    public class UserDTO
    {
        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; } = "";

        [MaxLength(100)]
        [Required]
        public string LastName { get; set; } = "";

        [MaxLength(100)]
        [Required]
        public string Email { get; set; } = "";
        [MaxLength(22)]
        public string? phone { get; set; } 
        [MaxLength(102) , Required]
        public string Address { get; set; } = "";
        [MaxLength(102) , MinLength(8) , Required]
        public string Password { get; set; } = "";
    }
}
