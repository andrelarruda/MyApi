using MyApi.Models;
using MyApi.Models.DTOs;

namespace MyApi.Interfaces
{
    public interface IAuthService
    {
        public Task<User> InsertUserAsync(UserRegisterDto user);
        public Task<string> LoginAsync(UserLoginDto userLoginDto);
    }
}
