using Authentication.Data;
using Authentication.Dto;
using Authentication.Entity;
using Authentication.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext context;
        private readonly IConfiguration configuration;
        public UserController(UserDbContext _context, IConfiguration _configuration)
        {
            context = _context;
            configuration = _configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {

            var user = await context.User.AnyAsync(u => u.Email == userRegisterDto.Email);

            if (user)
            {
                return BadRequest("User with this email already exists");
            }

            var newUser = new User
            {
                Username = userRegisterDto.Username,
                Email = userRegisterDto.Email,
                Balance = userRegisterDto.Balance,
                Role = UserRole.User
            };

            newUser.PasswordHash = new PasswordHasher<User>().HashPassword(newUser, userRegisterDto.Password);

            context.Add(newUser);
            await context.SaveChangesAsync();

            return Ok(newUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {

            var user = await context.User.FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);

            if (user is null)
            {
                return BadRequest("Invalid email or password");
            }

            var isPasswordValid = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userLoginDto.Password);

            if (isPasswordValid == PasswordVerificationResult.Failed)
            {
                return BadRequest("Invalid email or password");
            }

            // Generate Jwt token
            string token = CreateJwtToken(user);

            //return Ok(token); // returning in token as string format
            return Ok(new
            {
                token
            }); // returning in JSON format
        }

        [HttpGet("home")]
        [Authorize]
        public IActionResult Home()
        {
            return Ok("Welcome to the home page");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return Ok("Welcome to the admin page");
        }
        private string CreateJwtToken(User user)
        {

            var claim = new List<Claim>{
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                        new Claim(ClaimTypes.Role,user.Role.ToString())
                    };

            // Validate configuration values to avoid passing null into Encoding.GetBytes
            var securityKey = configuration.GetValue<string>("TokenDetail:SecurityKey");
            if (string.IsNullOrWhiteSpace(securityKey))
            {
                throw new InvalidOperationException("Configuration value 'TokenDetail:SecurityKey' is missing or empty.");
            }

            var issuer = configuration.GetValue<string>("TokenDetail:Issuer");
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new InvalidOperationException("Configuration value 'TokenDetail:Issuer' is missing or empty.");
            }

            var audience = configuration.GetValue<string>("TokenDetail:Audience");
            if (string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException("Configuration value 'TokenDetail:Audience' is missing or empty.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claim,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var JwtToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return JwtToken;
        }
    }
}
