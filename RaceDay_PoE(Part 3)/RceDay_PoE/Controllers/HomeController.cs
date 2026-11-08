using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using RaceDay_PoE.Models;
using System.Text.Json;

namespace RaceDayMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Home/Index
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // We call the public GET events endpoint
            var response = await client.GetAsync("api/events");

            var events = new List<AuthDtos.EventDto>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                // API returns { events: [...], total_count: X }
                var wrapper = JsonSerializer.Deserialize<AuthDtos.EventListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                events = wrapper?.Events ?? new List<AuthDtos.EventDto>();
            }

            return View(events);
        }

        // GET: /Home/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // 1. Get the event details (which includes categories in your API)
            var response = await client.GetAsync($"api/events/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            // API returns { event: {...}, categories: [...], weather: {...} }
            // We need a wrapper class for this. 
            var wrapper = JsonSerializer.Deserialize<EventDtos.EventDetailsResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(wrapper);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}