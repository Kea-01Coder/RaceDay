using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/results/{id}
        // Get a specific result. Public.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResult(int id)
        {
            var result = await _context.Results
                .Include(r => r.Enrollment)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null)
                return NotFound(new { message = $"Result {id} not found." });

            return Ok(result);
        }

        // PUT: api/results/{id}
        // Update a result. Organiser only.
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> UpdateResult(int id, [FromBody] Result updated)
        {
            var existing = await _context.Results.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Result {id} not found." });

            existing.FinishTime = updated.FinishTime;
            existing.Position = updated.Position;
            existing.OverallRank = updated.OverallRank;
            existing.CategoryRank = updated.CategoryRank;
            existing.Certified = updated.Certified;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/results/{id}
        // Delete a result. Organiser only.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null)
                return NotFound(new { message = $"Result {id} not found." });

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Result deleted." });
        }
    }
}
