using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApi.Context;
using MyApi.Interfaces;
using MyApi.Models;
using MyApi.Models.DTOs;
using BC = BCrypt.Net.BCrypt;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase, IAuthController
    {
        private readonly MyApiContext _context;

        public AuthController(MyApiContext context)
        {
            _context = context;
        }

        [HttpPost("registrate")]
        public async Task<ActionResult<User>> Registrate(UserRegisterDto requestData)
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

                string hashedPassword = BC.HashPassword(requestData.Password);
                User userToInsert = new User
                {
                    FirstName = requestData.FirstName,
                    LastName = requestData.LastName,
                    Email = requestData.Email,
                    IsActive = true,
                    PasswordHash = hashedPassword,
                };
                var result = await _context.Users.AddAsync(userToInsert);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Registrate), new { Message = "User created successfully." } );
            }  
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        public async Task<ActionResult> Login(UserLoginDto user)
        {
            // TODO: refactor it using a service
            if (user.Email.IsNullOrEmpty() || user.Password.IsNullOrEmpty())
            {
                throw new Exception("Please fill out all fields.");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if(existingUser == null)
            {
                return NotFound("User not found.");
            }

            if(!BC.Verify(BC.HashPassword(user.Password), existingUser.PasswordHash))
            {
                return BadRequest("Email or password invalid.");
            } else
            {
                // TODO: return a token
                return Ok("User logged!");
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
