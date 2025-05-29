using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LottoCheckerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LottoCheckerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetWinningSuperLottoDrawsForUser")]
        public async Task<ActionResult> GetAsync(string userId)
        {
            try
            {
                var winningNumbers = await _context.WinningPicksFromProc
                    .FromSqlInterpolated($"EXEC LottoChecker.GetWinningSuperLottoDrawsForUser @UserId = {userId}")
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(winningNumbers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("CreateSuperLottoPickForUser")]
        public async Task<ActionResult> PostAsync([FromBody] SuperLottoUserPick superLottoUserPick)
        {
            try
            {
                _context.SuperLottoUserPicks.Add(superLottoUserPick);
                await _context.SaveChangesAsync();
                return Ok(superLottoUserPick.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("DeleteSuperLottoPickForUser")]
        public async Task<ActionResult> DeleteAsync(Guid pickId)
        {
            try
            {
                var superLottoUserPick = await _context.SuperLottoUserPicks
                    .FindAsync(pickId) ?? throw new Exception("Cannot find user pick to delete");

                _context.Remove(superLottoUserPick);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("UpdateSuperLottoPickForUser")]
        public async Task<ActionResult> UpdateAsync([FromBody] SuperLottoUserPick superLottoUserPickModified)
        {
            try
            {
                var superLottoUserPick = await _context.SuperLottoUserPicks
                    .FindAsync(superLottoUserPickModified.Id) ?? throw new Exception("Cannot find user pick to update");

                superLottoUserPick.UserId = superLottoUserPickModified.UserId;
                superLottoUserPick.FirstPick = superLottoUserPickModified.FirstPick;
                superLottoUserPick.SecondPick = superLottoUserPickModified.SecondPick;
                superLottoUserPick.ThirdPick = superLottoUserPickModified.ThirdPick;
                superLottoUserPick.FourthPick = superLottoUserPickModified.FourthPick;
                superLottoUserPick.FifthPick = superLottoUserPickModified.FifthPick;
                superLottoUserPick.MegaPick = superLottoUserPickModified.MegaPick;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("GetSuperLottoPicksForUser")]
        public async Task<ActionResult> GetAsyncUserPicks(string userId)
        {
            try
            {
                var superLottoUserPicks = await _context.SuperLottoUserPicks
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(x => x.DateAdded)
                    .ToListAsync();

                return Ok(superLottoUserPicks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
