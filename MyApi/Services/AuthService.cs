using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MyApi.Context;
using MyApi.Interfaces;
using MyApi.Models;
using MyApi.Models.DTOs;
using BC = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;

namespace MyApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly MyApiContext _context;
        private readonly IMapper _mapper;

        public AuthService(MyApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<User> InsertUserAsync(UserRegisterDto userDto)
        {
            string hashedPassword = BC.HashPassword(userDto.Password);
            
            User user = _mapper.Map<User>(userDto);
            user.PasswordHash = hashedPassword;

            var result = await _context.Users.AddAsync(user); // use AuthService.InsertUser
            if (result.State == EntityState.Added)
            {
                await _context.SaveChangesAsync();
                return user;
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

            if (!BC.Verify(BC.HashPassword(userLoginDto.Password), existingUser.PasswordHash))
            {
                throw new ArgumentOutOfRangeException("Email or password invalid.");
            } else
            {
                // TODO: generate and return a token from GetToken()
                return string.Empty;
            }
        }
    }
}
