using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RaceDay.Controllers;
using RaceDay.Models;
using RaceDay.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Tests
{
    public class AuthControllerTests
    {
        private IConfiguration GetMockConfig()
        {
            // Mock configuration for JWT
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "THIS_IS_A_VERY_LONG_SECRET_KEY_FOR_TESTING_12345"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };
            return new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        }

        [Fact]
        public async Task Register_NewUser_ReturnsCreated()
        {
            // Arrange
            var context = TestDbHelper.GetInMemoryDb();
            var controller = new AuthController(context, GetMockConfig());
            var request = new RegisterRequest
            {
                Email = "test@test.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User",
                Role = "Participant",
                Phone = "0821234567",
                IDNumber = "9001015009087" 
            };

            // Act
            var result = await controller.Register(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // Expect ObjectResult
            Assert.Equal(201, objectResult.StatusCode);             // Check the 201
            Assert.Single(context.Users); // Check DB has 1 user
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            // Arrange
            var context = TestDbHelper.GetInMemoryDb();
            // Seed a user with a known hashed password
            context.Users.Add(new User
            {
                Email = "login@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
                Role = "Participant",
                FirstName = "Test", 
                LastName = "User"   
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(context, GetMockConfig());
            var request = new LoginRequest { Email = "login@test.com", Password = "WrongPassword!" };

            // Act
            var result = await controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result); // Expects 401
        }
    }
}
