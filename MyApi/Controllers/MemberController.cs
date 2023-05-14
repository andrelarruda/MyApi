using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApi.Context;
using MyApi.Models;
using MyApi.Models.DTOs;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly MyApiContext _context;
        public MemberController(MyApiContext myApiContext)
        {   
            _context = myApiContext;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Member>> Create(MemberDto requestData)
        {
            if (!ModelState.IsValid)
            {
                // add errors to the response.
                return BadRequest("Foram encontrados erros nos dados informados.");
            }

            try
            {
                var insertedMember = await _context.Members.AddAsync(new Member
                {
                    FullName = requestData.FullName,
                    Birthdate = requestData.BirthDate,
                    RG = requestData.RG,
                    CPF = requestData.CPF,
                    Occupation = requestData.Occupation,
                });

                return Ok(insertedMember);

            } catch (Exception ex)
            {
                string message = ex.Message;
                return BadRequest( new { message });
            }

        }
    }
}
