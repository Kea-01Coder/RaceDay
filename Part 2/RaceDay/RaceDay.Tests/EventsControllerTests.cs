using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Controllers;
using RaceDay.Models;
using RaceDay.Tests.Helpers;
using System.Security.Claims;

namespace RaceDay.Tests
{
    public class EventsControllerTests
    {
        // Helper to fake a logged-in user
        private void SetControllerUser(ControllerBase controller, int userId, string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
               new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
               new Claim(ClaimTypes.Role, role)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
               HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task CreateEvent_AsOrganiser_ReturnsCreated()
        {
                // Arrange
                var context = TestDbHelper.GetInMemoryDb();
                var controller = new EventsController(context);
                SetControllerUser(controller, 1, "Organiser"); // Fake an Organiser

                var newEvent = new Event { Title = "Test Marathon"
                    , EventDateTime = DateTime.Now,
                    Location = "Cape Town",

                };

                // Act
                var result = await controller.CreateEvent(newEvent);

                // Assert
                Assert.IsType<StatusCodeResult>(result);
                Assert.Single(context.Events);
        }

        [Fact]
        public async Task GetUserEnrollments_ParticipantAccessingOtherUser_ReturnsForbid()
        {
                // Arrange
                var context = TestDbHelper.GetInMemoryDb();
                var controller = new UsersController(context);

                // Fake a Participant with ID 1
                SetControllerUser(controller, 1, "Participant");

                // Act
                // Try to view enrollments for User ID 2 (a different user)
                var result = await controller.GetUserEnrollments(2);

                // Assert
                Assert.IsType<ForbidResult>(result); // Proves Role Rejection works
        }

        [Fact]
        public async Task EnrollInEvent_ValidParticipant_ReturnsCreated()
        {
                // Arrange
                var context = TestDbHelper.GetInMemoryDb();
                var controller = new EnrollmentsController(context);
                SetControllerUser(controller, 1, "Participant");

                // Seed an event and category
                var ev = new Event { Title = "Test", 
                    MaxParticipants = 100,
                    Location = "Cape Town",
                };
                context.Events.Add(ev);
                await context.SaveChangesAsync();
                var cat = new Category { EventId = ev.EventId, Name = "10km", EntryFee = 100 };
                context.Categories.Add(cat);
                await context.SaveChangesAsync();

                var enrollment = new Enrollment
                {
                    CategoryId = cat.CategoryId,
                    ParticipantName = "John",
                    ParticipantIdNumber = "1234567890",
                    Status = "pending"
                };

                // Act
                var result = await controller.EnrollInEvent(ev.EventId, enrollment);

                // Assert
                Assert.IsType<StatusCodeResult>(result);
                Assert.Single(context.Enrollments); // Proves enrollment was recorded
        }
     
    }
}
