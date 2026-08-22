using BestStoreAPI.Dtos;
using BestStoreAPI.Models;
using BestStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BestStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public AccountController(IConfiguration configuration , ApplicationDbContext applicationDbContext)
        {
            _configuration = configuration;
            _context = applicationDbContext;
        }
        [HttpPost("Register")]
        public IActionResult Register(UserDTO userDTO)
        {
            //check if the email is use befor or not 
            var emailCount = _context.Users.Count(u=>u.Email == userDTO.Email);
            if (emailCount > 0)
            {
                ModelState.AddModelError("Email", "This Email address alredy used");
                return BadRequest(ModelState);
            }
            //encript the password 
            var passwordHasher = new PasswordHasher<User>();
            var encryptPassword = passwordHasher.HashPassword(new User(), userDTO.Password);

            //creat new account 
            User user = new User()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                Password = encryptPassword,
                phone = userDTO.phone?? "",
                Address = userDTO.Address,
                Role = "client",
                CreatedAT =     DateTime.UtcNow,
                

            };
            _context.Users.Add(user);
            _context.SaveChanges();
            var jwt = CreateJWTtoken(user);


            UserProfileDTO userProfileDTO = new UserProfileDTO()
            {
               Id = user.Id,
               FirstName = user.FirstName,
               LastName = user.LastName,
               Email = user.Email,
               phone =  user.phone,
               Address = user.Address,
               Role = user.Role,
               CreatedAT= DateTime.UtcNow,

            };
            var respnse = new
            {
                token = jwt,
                User = userProfileDTO
            };
            return Ok(respnse);

        }
        [HttpPost("Login")]
        public IActionResult Login(string email , string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if ( user == null)
            {
                ModelState.AddModelError("Error", "email or password not valid");
                return BadRequest(ModelState);
            }

            //verify the password 
            var passwordHash = new PasswordHasher<User>();
            var re = passwordHash.VerifyHashedPassword(new User(),user.Password,password);

            if(re == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Password", "wrong Password");
                return BadRequest(ModelState);

            }
            var jwt = CreateJWTtoken(user);
            UserProfileDTO userProfileDTO = new UserProfileDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                phone = user.phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAT = DateTime.UtcNow,

            };
            var respnse = new
            {
                token = jwt,
                User = userProfileDTO
            };
            return Ok(respnse);

        }

        [Authorize]
        [HttpGet("authorizeAuthounticationUsers")]
        public IActionResult authorizeAuthounticationUsers()
        {
            return Ok("you are authorized");
        }

        [Authorize(Roles = "admin")]
        [HttpGet("AuthorizeAdmin")]
        public IActionResult AuthorizeAdmin()
        {
            return Ok("you are authorized");
        }

        [Authorize(Roles = "admin , Seller")]
        [HttpGet("AuthorizeAdminAndSeller")]
        public IActionResult AuthorizeAdminAndSeller()
        {
            return Ok("you are authorized");
        }



        [Authorize]
        [HttpGet("getTokenClaims")]
        public IActionResult GetTokenClaims()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                Dictionary<string, string> claims = new Dictionary<string, string>();
                foreach (Claim claim in identity.Claims)
                {
                    claims.Add(claim.Type, claim.Value);
                    
                }
                return Ok(claims);
            }
            return Ok();
            


        }

        [HttpPost("ForgetPassword")]
        public IActionResult ForgetPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "This Email not found");
                return BadRequest(ModelState);
            }
            //generate new password 
            var newPassword = Guid.NewGuid().ToString().Substring(0, 8);
            //encript the password 
            var passwordHasher = new PasswordHasher<User>();
            var encryptPassword = passwordHasher.HashPassword(new User(), newPassword);
            user.Password = encryptPassword;
            _context.SaveChanges();
            //send the new password to the user email 
            //send email 
            return Ok(new { message = "New Password has been sent to your email", NewPassword = newPassword });
        }

        //[HttpPost("ForgetPassword")]
        //public IActionResult ForgotPassword(string email )
        //{ 
        //    var user = _context.Users.FirstOrDefault(x => x.Email == email);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError("Email", "This Email not found");
        //        return BadRequest(ModelState);
        //    }
        //    //delete any old password 
        //    var oldPasswordReset = _context.PasswordResets.FirstOrDefault(x => x.Email==email);
        //    if (oldPasswordReset != null)
        //    {
        //        _context.Remove(oldPasswordReset);

        //    }
        //    //create new password reset token
        //    var token = Guid.NewGuid().ToString() + "--" + Guid.NewGuid().ToString();
        //    var pwdReset = new PasswordReset()
        //    {
        //        Email = email,
        //        Token = token,
        //        CreatedAt = DateTime.UtcNow
        //    };
        //    _context.PasswordResets.Add(pwdReset);
        //    _context.SaveChanges();
        //    //send the password reset token by email to the user
        //    string emailSubject = "Password Reset Token";
        //    string username = user.FirstName + " " + user.LastName;
        //    string emailmessage = $"Hello {username},\n\nYou have requested to reset your password. Please use the following token to reset your password:\n\n{token}\n\nIf you did not request this, please ignore this email.\n\nBest regards,\nBestStore Team";




        //    return Ok(new { message = "Password reset token has been created", Token = token });
        //}
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {            
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userIdClaim = identity.FindFirst("id");
                if (userIdClaim != null)
                {
                    int userId = int.Parse(userIdClaim.Value);
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        UserProfileDTO userProfileDTO = new UserProfileDTO()
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            phone = user.phone,
                            Address = user.Address,
                            Role = user.Role,
                            CreatedAT = DateTime.UtcNow,
                        };
                        return Ok(userProfileDTO);
                    }
                }
            }
            return NotFound();
        }
        [Authorize]
        [HttpPut("UpdateProfile")]
        public IActionResult UpdateProfile(UserProfileDTO userProfileDTO)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userIdClaim = identity.FindFirst("id");
                if (userIdClaim != null)
                {
                    int userId = int.Parse(userIdClaim.Value);
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        user.FirstName = userProfileDTO.FirstName;
                        user.LastName = userProfileDTO.LastName;
                        user.phone = userProfileDTO.phone;
                        user.Address = userProfileDTO.Address;
                        _context.SaveChanges();
                        return Ok(new { message = "Profile updated successfully" });
                    }
                }
            }
            return NotFound();
        }



        [Authorize]
        [HttpPut("UpdatePassword")]
        public IActionResult UpdatePassword( [Required , MinLength(6) ,MaxLength(100)] string newPassword)
        {
            int userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var passwordHasher = new PasswordHasher<User>();
            string encryptPassword = passwordHasher.HashPassword(new User(), newPassword);

            //update the password in the database
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Password = encryptPassword;
                _context.SaveChanges();
                return Ok(new { message = "Password updated successfully" });
            }

            return NotFound();



        }















        private string CreateJWTtoken(User user )
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", "" + user.Id),
                new Claim("role", user.Role)

            };
            string strKey = _configuration["JwtSettings:Key"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(strKey));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims : claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials : creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;




        }
    }
}
