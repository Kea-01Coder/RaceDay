using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly RaceDayDbContext _context;

        public UsersController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // GET: api/users/me
        // Returns the logged-in user's own profile.
        // Any authenticated user can call this.
        // ============================================
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            // Extract the user ID from the JWT claim
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _context.Users
                .Where(u => u.UserID == userId)
                .Select(u => new
                {
                    u.UserID,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Role,
                    u.Phone,
                    u.IDNumber,
                    u.DateOfBirth,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        // ============================================
        // PUT: api/users/me
        // Updates the logged-in user's own profile.
        // Only specific fields can be changed — not role, not password.
        // ============================================
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest req)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Only update fields that were actually supplied
            if (!string.IsNullOrEmpty(req.FirstName)) user.FirstName = req.FirstName;
            if (!string.IsNullOrEmpty(req.LastName)) user.LastName = req.LastName;
            if (!string.IsNullOrEmpty(req.Email)) user.Email = req.Email;
            if (!string.IsNullOrEmpty(req.Phone)) user.Phone = req.Phone;

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // ============================================
        // GET: api/users/{id}/enrollments
        // Gets a user's enrollment history.
        // Only the user themselves OR an Admin can access this.
        // This is why we can't just use [Authorize] — we need
        // to compare the requested id with the logged-in user's id.
        // ============================================
        [HttpGet("{id}/enrollments")]
        [Authorize]
        public async Task<IActionResult> GetUserEnrollments(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Rule: only the user themselves or an Admin
            if (currentUserId != id && currentRole != "Admin")
                return Forbid();

            var enrollments = await _context.Enrollments
                .Where(en => en.UserId == id)
                .Include(en => en.Event)
                .Include(en => en.Category)
                .Select(en => new
                {
                    en.EnrollmentId,
                    EventTitle = en.Event.Title,
                    EventDate = en.Event.EventDateTime,
                    CategoryName = en.Category.Name,
                    en.Status,
                    en.EntryDate,
                    en.PaymentStatus
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // ============================================
        // GET: api/users/{id}/results
        // Gets a user's race results.
        // Only the user themselves OR an Admin can access this.
        // ============================================
        [HttpGet("{id}/results")]
        [Authorize]
        public async Task<IActionResult> GetUserResults(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId != id && currentRole != "Admin")
                return Forbid();

            var results = await _context.Results
                .Include(r => r.Enrollment)
                    .ThenInclude(en => en.Event)
                .Where(r => r.Enrollment.UserId == id)
                .Select(r => new
                {
                    r.ResultId,
                    EventTitle = r.Enrollment.Event.Title,
                    r.FinishTime,
                    r.Position,
                    r.OverallRank,
                    r.CategoryRank,
                    r.Certified
                })
                .ToListAsync();

            return Ok(results);
        }
    }

    // ============================================
    // DTO (Data Transfer Object) for updating profile — only includes editable fields.
    // Note: role, password hash, and IDs are NOT here — by design.
    // ============================================
    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
