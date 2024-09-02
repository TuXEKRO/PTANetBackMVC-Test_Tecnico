using interviewProject.Data;
using interviewProject.DTOs;
using interviewProject.Models;
using interviewProject.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace interviewProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MbaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MbaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetMbaByName(string name)
        {
            var result = await ResultExtensions.TryAsync(async () =>
            {
                var mba = await _context.Mbas.FirstOrDefaultAsync(m => m.Name == name);

                if (mba == null)
                {
                    return Result<MbaDto>.Failure("MBA not found");
                }

                var mbaDto = new MbaDto
                {
                    Code = mba.Code,
                    Name = mba.Name
                };

                return Result<MbaDto>.Success(mbaDto);
            });

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(result.Error);
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateMba(string name, [FromBody] MbaDto mbaDto)
        {
            var result = await ResultExtensions.TryAsync(async () =>
            {
                var mba = await _context.Mbas.FirstOrDefaultAsync(m => m.Name == name);

                if (mba == null)
                {
                    return Result.Failure("MBA not found");
                }

                mba.Code = mbaDto.Code;

                await _context.SaveChangesAsync();

                return Result.Success();
            });

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.Error);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteMba(string name)
        {
            var result = await ResultExtensions.TryAsync(async () =>
            {
                var mba = await _context.Mbas.FirstOrDefaultAsync(m => m.Name == name);

                if (mba == null)
                {
                    return Result.Failure("MBA not found");
                }

                _context.Mbas.Remove(mba);
                await _context.SaveChangesAsync();

                return Result.Success();
            });

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return NotFound(result.Error);
        }
    }
}
