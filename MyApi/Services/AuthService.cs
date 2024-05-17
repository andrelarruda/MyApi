using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MyApi.Context;
using MyApi.Interfaces;
using MyApi.Models;
using MyApi.Models.DTOs;
using BC = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace MyApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly MyApiContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(MyApiContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<UserRegisterDto> InsertUserAsync(UserRegisterDto userDto)
        {
            string hashedPassword = BC.HashPassword(userDto.Password);
            
            User user = _mapper.Map<User>(userDto);
            user.PasswordHash = hashedPassword;

            // verifies if user already exists
            if (_context.Users.Where(u => u.Email == user.Email).Any())
            {
                throw new ArgumentException("User already registered.");
            }

            var result = await _context.Users.AddAsync(user);
            if (result.State == EntityState.Added)
            {
                await _context.SaveChangesAsync();
                return userDto;
            } else
            {
                throw new IOException("Something went wrong while trying to save the user.");
            }
        }

        public async Task<string> LoginAsync(UserLoginDto userLoginDto)
        {
            if (userLoginDto.Email.IsNullOrEmpty() || userLoginDto.Password.IsNullOrEmpty())
            {
                throw new ArgumentOutOfRangeException("Please fill out all fields.");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);
            if (existingUser == null)
            {
                throw new IOException("User not found.");
            }

            if (!BC.Verify(userLoginDto.Password, existingUser.PasswordHash))
            {
                throw new ArgumentOutOfRangeException("Email or password invalid.");
            } else
            {
                string token = CreateToken(existingUser);
                return token;
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Role, "Admin"), // Provisory. Must allow the user to specify a role
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value!
                    ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
