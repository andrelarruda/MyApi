using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Context;
using MyApi.Interfaces;
using MyApi.Models;
using MyApi.Models.DTOs;
using MyApi.Models.Responses;

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
        [ProducesResponseType(typeof(ResponseUserRegisteredJson), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResponseUserRegisteredJson>> Register(UserRegisterDto requestData)
        {
            if (requestData == null)
            {
                return BadRequest("Something went wrong.");
            }

            try
            {
                if (!requestData.Password.Equals(requestData.PasswordConfirmation))
                {
                    return BadRequest("Passwords don't match.");
                }

                await _authService.InsertUserAsync(requestData);
                ResponseUserRegisteredJson resultObj = new() { Message = $"User {requestData.FirstName} has been inserted successfully." };

                return CreatedAtAction("register", resultObj);
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
