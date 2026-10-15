using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Data;
using RaceDay.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // PUBLIC ENDPOINTS
        // ============================================

        // GET: api/events?status=upcoming&location=Cape Town
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] string? status, [FromQuery] string? location)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status == status);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(e => e.Location.Contains(location));

            var events = await query
                .OrderBy(e => e.EventDateTime)
                .ToListAsync();

            return Ok(new { events, total_count = events.Count });
        }

        // GET: api/events/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null)
                return NotFound(new { message = $"Event {id} not found." });

            var weather = await _context.WeatherInfos
                .Where(w => w.EventId == id)
                .OrderByDescending(w => w.ForecastDate)
                .FirstOrDefaultAsync();

            return Ok(new { @event = ev, categories = ev.Categories, weather });
        }

        // GET: api/events/5/participants
        [HttpGet("{id}/participants")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> GetEventParticipants(int id)
        {
            var participants = await _context.Enrollments
                .Where(en => en.EventId == id)
                .Include(en => en.User)
                .Include(en => en.Category)
                .Select(en => new
                {
                    en.EnrollmentId,
                    en.ParticipantName,
                    en.ParticipantIdNumber,
                    en.BibNumber,
                    CategoryName = en.Category.Name,
                    UserEmail = en.User.Email
                })
                .ToListAsync();

            return Ok(participants);
        }

        // GET: api/events/5/results?category_id=1
        [HttpGet("{id}/results")]
        public async Task<IActionResult> GetEventResults(int id, [FromQuery] int? category_id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound(new { message = $"Event {id} not found." });

            var query = _context.Results
                .Include(r => r.Enrollment)
                .Where(r => r.Enrollment.EventId == id);

            if (category_id.HasValue)
                query = query.Where(r => r.Enrollment.CategoryId == category_id.Value);

            var results = await query
                .OrderBy(r => r.Position)
                .ToListAsync();

            return Ok(new { @event = ev, results });
        }

        // GET: api/events/5/weather
        [HttpGet("{id}/weather")]
        public async Task<IActionResult> GetEventWeather(int id)
        {
            var weather = await _context.WeatherInfos
                .Where(w => w.EventId == id)
                .OrderByDescending(w => w.ForecastDate)
                .FirstOrDefaultAsync();

            if (weather == null)
                return NotFound(new { message = $"No weather info for event {id}." });

            return Ok(weather);
        }

        // ============================================
        // ORGANISER ENDPOINTS
        // ============================================

        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            newEvent.CreatedBy = userId;
            newEvent.CreatedAt = DateTime.UtcNow;
            newEvent.Status = "upcoming";

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { event_id = newEvent.EventId });
        }

        // PUT: api/events/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updated)
        {
            var existing = await _context.Events.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Event {id} not found." });

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.EventDateTime = updated.EventDateTime;
            existing.Location = updated.Location;
            existing.MaxParticipants = updated.MaxParticipants;
            existing.EntryFee = updated.EntryFee;
            existing.Status = updated.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound(new { message = $"Event {id} not found." });

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Event deleted." });
        }

        // ============================================
        // NESTED: CATEGORIES
        // ============================================

        // POST: api/events/5/categories
        [HttpPost("{id}/categories")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> AddCategory(int id, [FromBody] Category newCategory)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound(new { message = $"Event {id} not found." });

            newCategory.EventId = id;
            newCategory.CreatedAt = DateTime.UtcNow;

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { category_id = newCategory.CategoryId });
        }

        // ============================================
        // NESTED: RESULTS (bulk upload)
        // ============================================

        // POST: api/events/5/results
        [HttpPost("{id}/results")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> SubmitResults(int id, [FromBody] List<Result> results)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound(new { message = $"Event {id} not found." });

            foreach (var r in results)
            {
                r.CreatedAt = DateTime.UtcNow;
                _context.Results.Add(r);
            }

            await _context.SaveChangesAsync();
            return StatusCode(201, new { results_count = results.Count });
        }

        // ============================================
        // NESTED: WEATHER (upsert)
        // ============================================

        // POST: api/events/5/weather
        [HttpPost("{id}/weather")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> UpsertWeather(int id, [FromBody] WeatherInfo weather)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound(new { message = $"Event {id} not found." });

            var existing = await _context.WeatherInfos
                .FirstOrDefaultAsync(w => w.EventId == id && w.ForecastDate == weather.ForecastDate);

            int weatherId;

            if (existing == null)
            {
                weather.EventId = id;
                weather.UpdatedAt = DateTime.UtcNow;
                _context.WeatherInfos.Add(weather);
                await _context.SaveChangesAsync();
                weatherId = weather.WeatherInfoId;
            }
            else
            {
                existing.Temperature = weather.Temperature;
                existing.Conditions = weather.Conditions;
                existing.WindSpeed = weather.WindSpeed;
                existing.Humidity = weather.Humidity;
                existing.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                weatherId = existing.WeatherInfoId;
            }

            return Ok(new { weather_id = weatherId });
        }

    }
}
