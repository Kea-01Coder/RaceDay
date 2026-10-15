using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;
using System.Security.Claims;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        private readonly RaceDayDbContext _context;

        public EnrollmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/events/{id}/enrol
        // A Participant enrols in an event by choosing a category.
        [HttpPost("/api/events/{id}/enrol")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> EnrollInEvent(int id, [FromBody] Enrollment enrolment)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Verify event exists
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound(new { message = $"Event {id} not found." });

            // Verify category exists and belongs to this event
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == enrolment.CategoryId && c.EventId == id);
            if (category == null)
                return BadRequest(new { message = "Invalid category for this event." });

            // Check the event isn't full
            var currentCount = await _context.Enrollments.CountAsync(en => en.EventId == id);
            if (currentCount >= ev.MaxParticipants)
                return BadRequest(new { message = "Event is full." });

            // Set server-controlled fields
            enrolment.UserId = userId;
            enrolment.EventId = id;
            enrolment.Status = "pending";
            enrolment.PaymentStatus = "pending";
            enrolment.EntryDate = DateTime.UtcNow;
            enrolment.AmountPaid = category.EntryFee;

            _context.Enrollments.Add(enrolment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // This fires if the unique index UC_Enrollment is violated
                return BadRequest(new { message = "You're already enrolled in this category for this event." });
            }

            return StatusCode(201, new
            {
                enrolment_id = enrolment.EnrollmentId,
                amount_due = category.EntryFee
            });
        }

        // GET: api/enrollments/{id}
        // Get a specific enrolment. Self (the enrolled user) or Admin only.
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var enrolment = await _context.Enrollments
                .Include(en => en.Event)
                .Include(en => en.Category)
                .FirstOrDefaultAsync(en => en.EnrollmentId == id);

            if (enrolment == null)
                return NotFound(new { message = $"Enrolment {id} not found." });

            if (enrolment.UserId != currentUserId && currentRole != "Admin")
                return Forbid();

            return Ok(enrolment);
        }

        // PUT: api/enrollments/{id}
        // Update enrolment details. Self or Admin only.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] Enrollment updated)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var existing = await _context.Enrollments.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Enrolment {id} not found." });

            if (existing.UserId != currentUserId && currentRole != "Admin")
                return Forbid();

            // Only allow updating participant-supplied details
            existing.ParticipantName = updated.ParticipantName;
            existing.EmergencyContactName = updated.EmergencyContactName;
            existing.EmergencyContactPhone = updated.EmergencyContactPhone;
            existing.MedicalConditions = updated.MedicalConditions;
            existing.CategoryId = updated.CategoryId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/enrollments/{id}
        // Cancel an enrolment. Self or Admin only. Returns refund status.
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> CancelEnrollment(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var enrolment = await _context.Enrollments.FindAsync(id);
            if (enrolment == null)
                return NotFound(new { message = $"Enrolment {id} not found." });

            if (enrolment.UserId != currentUserId && currentRole != "Admin")
                return Forbid();

            // Mark as cancelled rather than deleting — preserves history
            enrolment.Status = "cancelled";
            enrolment.UpdatedAt = DateTime.UtcNow;

            // Simple refund rule: if paid, mark as refunded
            var refundStatus = "none";
            if (enrolment.PaymentStatus == "paid")
            {
                enrolment.PaymentStatus = "refunded";
                refundStatus = "processed";
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Enrolment cancelled.", refund_status = refundStatus });
        }

        // POST: api/enrollments/{id}/pay
        // Process a payment. Participant only.
        // NOTE: This is a stub — no real payment gateway. Returns a fake reference.
        [HttpPost("{id}/pay")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> ProcessPayment(int id, [FromBody] PaymentRequest req)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var enrolment = await _context.Enrollments.FindAsync(id);
            if (enrolment == null)
                return NotFound(new { message = $"Enrolment {id} not found." });

            if (enrolment.UserId != userId)
                return Forbid();

            if (enrolment.PaymentStatus == "paid")
                return BadRequest(new { message = "Enrolment is already paid." });

            // Simulate a payment — generate fake identifiers
            var fakeRef = "PAY-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
            var fakeInvoice = "INV-" + DateTime.UtcNow.Year + "-" + id.ToString("D4");

            enrolment.PaymentStatus = "paid";
            enrolment.PaymentMethod = req.PaymentMethod;
            enrolment.PaymentRef = fakeRef;
            enrolment.InvoiceNumber = fakeInvoice;
            enrolment.PaymentDate = DateTime.UtcNow;
            enrolment.Status = "confirmed";
            enrolment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                payment_status = "paid",
                payment_ref = fakeRef,
                invoice_number = fakeInvoice
            });
        }
    }

    public class PaymentRequest
    {
        public string PaymentMethod { get; set; }
    }
}

