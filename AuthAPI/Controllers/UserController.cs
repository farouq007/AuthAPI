using AuthAPI.Data;
using AuthAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public static User _user = new();
        public static Password password = new();

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Post([FromBody] UserDTO user)
        {
            password.CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
            _user.FirstName = user.FirstName;
            _user.LastName = user.LastName;
            _user.Email = user.Email;
            _user.PhoneNumber = user.PhoneNumber;
            _user.DateCreated = DateTime.UtcNow;
            _user.LastLogin = DateTime.UtcNow;
            _user.PasswordHash = passwordHash;
            _user.PasswordSalt = passwordSalt;

            _context.Users.Add(_user);
            await _context.SaveChangesAsync();
            return Ok(_user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDTO userLogin)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userLogin.Email);
            if (user != null)
            {
                if (password.VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
                {
                    string token = password.CreateToken(user);
                    return Ok(token);
                }
                return BadRequest("Incorrect Password");
            }
            return BadRequest("User not found");
        }

        [HttpDelete("{email}")]
        public async Task<ActionResult<User>> DeleteUser(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok($"{user.FirstName}'s profile has been deleted");
            }
            return BadRequest($" {email} not found");
        }

        [HttpGet("AllUsers")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}