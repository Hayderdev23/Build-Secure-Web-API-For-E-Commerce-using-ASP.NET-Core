using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreAPI.Models
{
    [Index("Email" , IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; } = "";

        [MaxLength(100)]
        public string LastName { get; set; } = "";

        [MaxLength(100)]
        public string Email { get; set; } = "";
        [MaxLength(22)]
        public string phone { get; set; } = "";
        [MaxLength(102)]
        public string Address { get; set; } = "";  
        [MaxLength(102)]
        public string Password { get; set; } = "";
        [MaxLength(20)]
        public string Role { get; set; } = "";
        public DateTime CreatedAT { get; set; } = DateTime.Now;
    }
}
