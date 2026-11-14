using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay_PoE.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static RaceDay_PoE.Models.Dto;

namespace RaceDay_PoE.Controllers
{
    [Authorize(Roles = "Organiser,Admin")]
    public class OrganiserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrganiserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Organiser/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var token = User.FindFirst("JWT")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Call the API to get all events
            var response = await client.GetAsync("api/events");

            var events = new List<EventDto>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var wrapper = JsonSerializer.Deserialize<EventListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                events = wrapper?.Events ?? new List<EventDto>();
            }

            return View(events);
        }

        // GET: /Organiser/CreateEvent
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        // POST: /Organiser/CreateEvent
        [HttpPost]
        public async Task<IActionResult> CreateEvent(Dto.CreateEventRequest model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var token = User.FindFirst("JWT")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Call the API's Create Event endpoint
            var response = await client.PostAsync("api/events", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Failed to create event. Please check your inputs.");
            return View(model);
        }

        // GET: /Organiser/ResultsEntry/5 (Where 5 is the Event ID)
        [HttpGet]
        public IActionResult ResultsEntry(int id)
        {
            // We pass the event ID to the view so the form knows where to submit
            ViewBag.EventId = id;
            return View();
        }

        // POST: /Organiser/ResultsEntry/5
        [HttpPost]
        public async Task<IActionResult> ResultsEntry(int id, SubmitResultRequest model)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var token = User.FindFirst("JWT")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // The API expects a LIST of results (bulk upload), so we put our single result in a list
            var payload = new List<SubmitResultRequest> { model };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"api/events/{id}/results", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Result added successfully!";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Failed to submit result. Check the Enrollment ID.");
            ViewBag.EventId = id;
            return View(model);
        }
    }
}