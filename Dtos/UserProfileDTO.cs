using System.ComponentModel.DataAnnotations;

namespace BestStoreAPI.Dtos
{
    public class UserProfileDTO
    {
        public int Id { get; set; }

      
        public string FirstName { get; set; } = "";

      
        public string LastName { get; set; } = "";

       
        public string Email { get; set; } = "";
      
        public string phone { get; set; } = "";
       
        public string Address { get; set; } = "";
        
       
        public string Role { get; set; } = "";
        public DateTime CreatedAT { get; set; } = DateTime.Now;
    }
}
