using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RaceDay.Data;
using RaceDay.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly RaceDayDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(RaceDayDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == req.Email))
                return BadRequest(new { message = "Email already registered." });

            // Validate role
            if (req.Role != "Organiser" && req.Role != "Participant" && req.Role != "Admin")
                return BadRequest(new { message = "Invalid role." });

            // Create the user
            var user = new User
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                FirstName = req.FirstName,
                LastName = req.LastName,
                Role = req.Role,
                Phone = req.Phone,
                IDNumber = req.IDNumber,
                DateOfBirth = req.DateOfBirth,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "User registered successfully.", userId = user.UserID });
        }

        // POST: auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials." });

            // Generate JWT
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserID,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role
                }
            });
        }

        // Helper: generate a JWT for the given user
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Request DTOs — kept inside the file for simplicity
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string? Phone { get; set; }
        public string? IDNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
