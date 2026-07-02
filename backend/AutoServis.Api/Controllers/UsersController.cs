using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoServis.Api.Data;
using AutoServis.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AutoServis.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<object>> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Ime, email i lozinka su obavezni." });
            }

            if (request.Password.Length < 6)
            {
                return BadRequest(new { message = "Lozinka mora imati najmanje 6 karaktera." });
            }

            var normalizedEmail = request.Email.Trim().ToLower();

            var userExists = await _context.Users.AnyAsync(user => user.Email == normalizedEmail);

            if (userExists)
            {
                return BadRequest(new { message = "Korisnik sa ovom email adresom već postoji." });
            }

            var user = new User
            {
                Name = request.Name.Trim(),
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "user"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(ToUserResponse(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email i lozinka su obavezni." });
            }

            var normalizedEmail = request.Email.Trim().ToLower();

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == normalizedEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Pogrešan email ili lozinka." });
            }

            return Ok(ToUserResponse(user));
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<object>> GetProfile()
        {
            var user = await GetCurrentUser();

            if (user == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            return Ok(ToUserResponse(user));
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<object>> UpdateProfile(UpdateProfileRequest request)
        {
            var user = await GetCurrentUser();

            if (user == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Ime i prezime su obavezni." });
            }

            user.Name = request.Name.Trim();
            user.Phone = request.Phone;
            user.City = request.City;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password.Length < 6)
                {
                    return BadRequest(new { message = "Nova lozinka mora imati najmanje 6 karaktera." });
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync();

            return Ok(ToUserResponse(user));
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            var users = await _context.Users
                .OrderBy(user => user.Id)
                .Select(user => new
                {
                    _id = user.Id,
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    role = user.Role,
                    phone = user.Phone,
                    city = user.City
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Korisnik nije pronađen." });
            }

            if (user.Role == "admin")
            {
                return BadRequest(new { message = "Administrator ne može biti obrisan." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Korisnik je obrisan." });
        }

        private async Task<User?> GetCurrentUser()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return null;
            }

            return await _context.Users.FindAsync(userId);
        }

        private object ToUserResponse(User user)
        {
            return new
            {
                _id = user.Id,
                id = user.Id,
                name = user.Name,
                email = user.Email,
                role = user.Role,
                phone = user.Phone,
                city = user.City,
                token = GenerateToken(user)
            };
        }

        private string GenerateToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key nije podešen.");
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class UpdateProfileRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? City { get; set; }

        public string? Password { get; set; }
    }
}