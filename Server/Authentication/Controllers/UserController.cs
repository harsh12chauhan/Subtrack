using Authentication.Dto;
using Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("auth")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService authService;
        public UserController(IAuthService _authService)
        {
            authService = _authService;
            configuration = _configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            var response = await authService.CreateNewUser(userRegisterDto);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var response = await authService.AuthenticateUser(userLoginDto);

            return Ok(response);
        }
    }
}
