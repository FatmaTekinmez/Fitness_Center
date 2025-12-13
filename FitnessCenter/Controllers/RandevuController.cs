using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    public class RandevuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
