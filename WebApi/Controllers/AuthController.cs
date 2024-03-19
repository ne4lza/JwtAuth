using Application.Contracts;
using Application.DTOs;
using Application.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUser user;
        public AuthController(IUser user)
        {
            this.user = user;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginUserDTO loginUserDTO)
        {
            
            var result = await user.LoginUserAsync(loginUserDTO);
            if(result.flag)
                return Ok(result);

            return BadRequest(result.message);
        }
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register(RegisterUserDTO registerUserDTO)
        {
            var result = await user.RegisterUserAsync(registerUserDTO);
            return Ok(result);
        }
        [HttpGet("getAuthUser"),Authorize]
        public async Task<ActionResult<LoginResponse>> GetAuthUser()
        {
            return Ok();
        }

    }
}
