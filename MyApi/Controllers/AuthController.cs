﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly IAuthService _authService;

        public AuthController(MyApiContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
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
                    return BadRequest("Passwords doesn't match.");
                }

                var userInserted = _authService.InsertUserAsync(requestData);

                return CreatedAtAction(nameof(Registrate), new { Message = "User created successfully." } );
            }  
            catch (IOException ex)
            {
                return Problem(ex.Message);
            }

        }

        public async Task<ActionResult> Login(UserLoginDto user)
        {
            try
            {
                string token = await _authService.LoginAsync(user);
                return Ok("User logged!");

            } catch (IOException ex)
            {
                return NotFound(ex.Message);
            } catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
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
