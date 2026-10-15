using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // PUT: api/categories/{id}
        // Update an existing category. Organiser only.
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category updated)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Category {id} not found." });

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.EntryFee = updated.EntryFee;
            existing.StartTime = updated.StartTime;
            existing.DistanceKM = updated.DistanceKM;
            existing.Capacity = updated.Capacity;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/categories/{id}
        // Delete a category. Organiser only. Will cascade-delete its enrolments.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { message = $"Category {id} not found." });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category deleted." });
        }
    }
}

