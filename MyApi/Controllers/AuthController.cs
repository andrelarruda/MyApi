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
        public async Task<ActionResult> Registrate(UserRegisterDto requestData)
        {
            if (requestData == null)
            {
                return BadRequest("Something went wrong.");
            }
            return Ok();
        }

        [HttpGet("test")]
        public async Task<ActionResult<User>> Test()
        {
            var test = await _context.Users.ToArrayAsync();
            return Ok(test);
        }
    }
}
