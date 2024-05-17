using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Context;
using MyApi.Interfaces;
using MyApi.Models;
using MyApi.Models.DTOs;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase, IAuthController
    {
        private readonly MyApiContext _context;
        private readonly IAuthService _authService;

        public AuthController(MyApiContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> Register(UserRegisterDto requestData)
        {
            if (requestData == null)
            {
                return BadRequest("Something went wrong.");
            }

            try
            {
                if (!requestData.Password.Equals(requestData.PasswordConfirmation))
                {
                    return BadRequest("Passwords doesn't match.");
                }

                var userInserted = await _authService.InsertUserAsync(requestData);

                return CreatedAtAction("register", userInserted);
            }  
            catch (IOException ex)
            {
                return Problem(ex.Message);
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto user)
        {
            try
            {
                string token = await _authService.LoginAsync(user);
                return Ok(new { message = "success", token });

            } catch (IOException ex)
            {
                return NotFound(ex.Message);
            } catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest("Email or password invalid.");
            }
        }

        [HttpGet("test")]
        public async Task<ActionResult<ICollection<User>>> Test()
        {
            List<User> result = await _context.Users.ToListAsync();
            return Ok(result);
        }
    }
}
