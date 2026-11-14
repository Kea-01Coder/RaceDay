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
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest req)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Category {id} not found." });

            // Map the DTO fields
            existing.Name = req.Name;
            existing.Description = req.Description;
            existing.EntryFee = req.EntryFee;
            existing.StartTime = req.StartTime;
            existing.DistanceKM = req.DistanceKM;
            existing.Capacity = req.Capacity;

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
        public class UpdateCategoryRequest
        {
            public string Name { get; set; }
            public string? Description { get; set; }
            public decimal EntryFee { get; set; }
            public TimeOnly? StartTime { get; set; }
            public decimal DistanceKM { get; set; }
            public int? Capacity { get; set; }
        }
    }
}

