using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay_PoE.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RaceDay_PoE.Controllers
{
    [Authorize(Roles = "Participant")] // Only participants can access this
    public class ParticipantController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ParticipantController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // Get the JWT token from the cookie
            var token = User.FindFirst("JWT")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Build the JSON body your API expects
            var payload = new
            {
                categoryId = categoryId,
                participantName = User.Identity.Name, // Using their name from the cookie
                participantIdNumber = "1234567890", // Placeholder - would come from a form in a real app
                participantDob = "1990-01-01",
                gender = "Male",
                emergencyContactName = "Emergency",
                emergencyContactPhone = "0821234567",
                medicalConditions = "None"
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Call your Part 2 API's Enrol endpoint
            var response = await client.PostAsync($"api/events/{eventId}/enrol", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Successfully enrolled!";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Could not enrol. You might already be enrolled.";
            return RedirectToAction("Details", "Home", new { id = eventId });
        }

        // GET: /Participant/MyEnrolments
        public async Task<IActionResult> MyEnrolments()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var token = User.FindFirst("JWT")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Call the API to get the user's enrollments
            var response = await client.GetAsync("api/enrollments");

            // For now, let's assume it returns a list of enrollments
            var enrollments = new List<dynamic>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
            }

            return View(enrollments);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
            {
                TempData["Error"] = "Please select a file.";
                return RedirectToAction("Profile");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var token = User.FindFirst("JWT")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var form = new MultipartFormDataContent();
            using var fileStream = profilePicture.OpenReadStream();
            form.Add(new StreamContent(fileStream), "file", profilePicture.FileName);

            var response = await client.PostAsync("api/users/me/upload-profile-picture", form);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Profile picture updated!";
            else
                TempData["Error"] = "Failed to upload picture.";

            return RedirectToAction("Profile");
        }
    }
}
