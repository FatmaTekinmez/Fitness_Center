using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    public class SalonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
