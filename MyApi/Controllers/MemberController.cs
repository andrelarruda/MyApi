using AutoMapper;
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
        private readonly IMapper _mapper;
        public MemberController(MyApiContext myApiContext, IMapper mapper)
        {   
            _context = myApiContext;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<ActionResult<MemberDto>> Create(MemberDto requestData)
        {
            if (!ModelState.IsValid)
            {
                // add errors to the response.
                return BadRequest("Foram encontrados erros nos dados informados.");
            }

            try
            {
                Member member = _mapper.Map<Member>(requestData);
                var insertedMember = await _context.Members.AddAsync(member);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<MemberDto>(insertedMember.Entity);
                return Ok(result);

            } catch (Exception ex)
            {
                string message = ex.Message;
                return BadRequest( new { message });
            }

        }

        [HttpGet("list")]
        public async Task<ActionResult<ICollection<MemberDto>>> ListAll()
        {
            try
            {
                var members = await _context.Members.ToListAsync<Member>();
                var result = _mapper.Map<ICollection<MemberDto>>(members);

                return Ok(result);
            } catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("edit")]
        public async Task<ActionResult<MemberDto>> Update(int id, MemberDto requestData)
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

            var result = _mapper.Map<MemberDto>(member);

            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<MemberDto>> Delete(int id)
        {
            var userToDelete = await _context.Members.FindAsync(id);

            if (userToDelete == null)
            {
                return NotFound("Member not found.");
            }

            var removed = _context.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<MemberDto>(removed.Entity));
        }

    }
}
