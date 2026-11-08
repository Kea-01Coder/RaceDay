using Microsoft.AspNetCore.Mvc;

namespace RaceDay_PoE.Controllers
{
    public class OrganiserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
