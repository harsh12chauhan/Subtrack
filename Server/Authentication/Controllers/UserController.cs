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
        public UserController(UserDbContext _context, IConfiguration _configuration  )
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

            return Ok(token);
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
        private string CreateJwtToken(User user) {

            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("TokenDetail:SecurityKey")));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("TokenDetail:Issuer"),
                audience: configuration.GetValue<string>("TokenDetail:Audience"),
                claims: claim,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var JwtToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return JwtToken;
        }
    }
}
