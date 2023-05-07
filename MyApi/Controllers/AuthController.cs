using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Context;
using MyApi.Models;
using MyApi.Models.DTOs;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private User _user;
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
                User userToInsert = new User
                {
                    FirstName = requestData.FirstName,
                    LastName = requestData.LastName,
                    Email = requestData.Email,
                    IsActive = true,
                    PasswordHash = "passwordhashtest",
                };
                var result = await _context.Users.AddAsync(userToInsert);

                return Ok(result);
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
