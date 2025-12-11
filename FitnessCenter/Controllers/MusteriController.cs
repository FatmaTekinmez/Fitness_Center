using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    public class MusteriController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
