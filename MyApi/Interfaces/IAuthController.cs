using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Models.DTOs;

namespace MyApi.Interfaces
{
    public interface IAuthController
    {
        Task<ActionResult> Login(UserLoginDto user);
        Task<ActionResult<User>> Register(UserRegisterDto user);
    }
}
