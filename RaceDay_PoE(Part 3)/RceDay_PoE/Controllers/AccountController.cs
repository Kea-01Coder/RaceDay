using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RaceDay_PoE.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static RaceDay_PoE.Models.Dto;

namespace RaceDay_PoE.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Call the API's login endpoint
            var response = await client.PostAsync("api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // 1. Create the claims (who is this user?)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.User.FirstName),
                    new Claim(ClaimTypes.Role, result.User.Role),
                    new Claim("JWT", result.Token) // We store the actual JWT here to use later!
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 2. Sign the user in (this creates the cookie)
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // 3. Redirect based on role
                if (result.User.Role == "Organiser" || result.User.Role == "Admin")
                    return RedirectToAction("Dashboard", "Organiser");
                else
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt. Please check your email and password.");
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed. The email may already be in use.");
            return View(model);
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
