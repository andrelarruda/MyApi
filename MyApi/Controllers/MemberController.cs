using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var member = new Member
                {
                    FullName = requestData.FullName,
                    Birthdate = requestData.Birthdate,
                    RG = requestData.RG,
                    CPF = requestData.CPF,
                    Occupation = requestData.Occupation,
                };
                var insertedMember = await _context.Members.AddAsync(member);
                await _context.SaveChangesAsync();

                // TODO: return a MemberDto instance instead.
                return Ok(insertedMember.Entity);

            } catch (Exception ex)
            {
                string message = ex.Message;
                return BadRequest( new { message });
            }

        }

        [HttpGet("list")]
        public async Task<ActionResult<ICollection<Member>>> ListAll()
        {
            try
            {
                var members = await _context.Members.ToListAsync<Member>();

                return Ok(members);
            } catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("edit")]
        public async Task<ActionResult<Member>> Update(int id, MemberDto requestData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Check the data informed and try again.");
            }

            var member = await _context.Members.FindAsync(id);

            if(member.Equals(null))
            {
                return NotFound("Member wasn't found.");
            }

            member.FullName = requestData.FullName != member.FullName ? requestData.FullName : member.FullName;
            member.CPF = requestData.CPF != member.CPF ? requestData.CPF : member.CPF;
            member.RG = requestData.RG != member.RG ? requestData.RG : member.RG;
            member.Birthdate = requestData.Birthdate != member.Birthdate ? requestData.Birthdate : member.Birthdate;
            member.Occupation = requestData.Occupation != member.Occupation ? requestData.Occupation : member.Occupation;
            member.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            // TODO: return a MemberDto instance instead.
            return Ok(member);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<Member>> Delete(int id)
        {
            var userToDelete = await _context.Members.FindAsync(id);

            if (userToDelete == null)
            {
                return NotFound("Member not found.");
            }

            var removed = _context.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return Ok(removed.Entity);
        }

    }
}
