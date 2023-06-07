using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Context;
using MyApi.Models;
using MyApi.Models.DTOs;
using BC = BCrypt.Net.BCrypt;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
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

        [HttpGet("test")]
        public async Task<ActionResult<ICollection<User>>> Test()
        {
            List<User> result = await _context.Users.ToListAsync();
            return Ok(result);
        }
    }
}
