using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessCenter.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext context;

        public AdminController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageGymCenters()
        {
            return View();
        }
        public IActionResult GetAllServices()
        {
            var services = context.Services.ToList();

            return View(services);
        }
        [HttpGet]
        public IActionResult GetAllGymCenters()
        {
            var gymCenters = context.GymCenters.ToList();
            return View(gymCenters);
        }
        [HttpGet]
        public IActionResult CreateGymCenter()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult CreateGymCenter(GymCenter gymCenter)
        {
            if (ModelState.IsValid)
            {
                context.GymCenters.Add(gymCenter);
                context.SaveChanges();
                return RedirectToAction("GetAllGymCenters");
            }
            return View(gymCenter);
        }
        [HttpGet]
        public IActionResult CreateService()
        {
            var gymCenters = context.GymCenters.ToList();
            ViewBag.GymCenters = new SelectList(gymCenters, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult CreateService(Service service)
        {
            if (ModelState.IsValid)
            {
                context.Services.Add(service);
                context.SaveChanges();
                return RedirectToAction("GetAllServices");
            }
            var gymCenters = context.GymCenters.ToList();
            ViewBag.GymCenters = new SelectList(gymCenters, "Id", "Name");
            return View(service);

        }
    }
}
