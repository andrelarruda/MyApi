using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MyApi.Context;
using MyApi.Interfaces;
using MyApi.Models;
using MyApi.Models.DTOs;
using BC = BCrypt.Net.BCrypt;

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
    }
}
