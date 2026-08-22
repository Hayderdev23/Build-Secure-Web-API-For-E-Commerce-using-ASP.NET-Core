using BestStoreAPI.Dtos;
using BestStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;  // we call it Dependency Injection Field
     
        public UserController(ApplicationDbContext context)
        {
            _context = context;
            
        }

        [HttpGet]
        public IActionResult GetUsers(int? page)
        {
            if (page == null || page < 1)
            { page = 1; }

            int pageSize = 10; // Number of users per page
            int totalpages = 0;
            totalpages = (int)Math.Ceiling((double)_context.Users.Count() / pageSize);
            var userss = _context.Users
                .OrderByDescending(u => u.Id)
                .Skip((page.Value - 1) * pageSize)
                .Take(pageSize)
                .ToList();



            var users = _context.Users.OrderByDescending(u=>u.Id) 
                .ToList();
            List<UserProfileDTO> userProfiles = new List<UserProfileDTO>();
            foreach (var user in users)
            {
                var userProfileDTO = new UserProfileDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    phone = user.phone,
                    Address = user.Address,
                    Role = user.Role,
                    CreatedAT = user.CreatedAT
                };
                userProfiles.Add(userProfileDTO);
            }

            var response = new
            {
                TotalPages = totalpages,
                CurrentPage = page,
                Users = userProfiles
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(o=>o.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var userProfileDTO = new UserProfileDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                phone = user.phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAT = user.CreatedAT
            };


            return Ok(userProfileDTO);

        }




    }
}
