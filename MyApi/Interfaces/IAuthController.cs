using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Models.DTOs;
using MyApi.Models.Responses;

namespace MyApi.Interfaces
{
    public interface IAuthController
    {
        Task<ActionResult> Login(UserLoginDto user);
        Task<ActionResult<ResponseUserRegisteredJson>> Register(UserRegisterDto user);
    }
}
