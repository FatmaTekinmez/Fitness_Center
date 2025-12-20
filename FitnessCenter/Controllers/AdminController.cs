using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessCenter.Controllers
{
    [Authorize(Roles = "Admin")]  // 🔒 sadece Admin rolü
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // ANA PANEL
        // =========================
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageGymCenters()
        {
            return View();
        }

        // =========================
        // SPOR SALONLARI (GYMCENTER)
        // =========================

        [HttpGet]
        public IActionResult GetAllGymCenters()
        {
            var gymCenters = _context.GymCenters.ToList();
            return View(gymCenters);   // Views/Admin/GetAllGymCenters.cshtml
        }

        [HttpGet]
        public IActionResult CreateGymCenter()
        {
            return View();             // Views/Admin/CreateGymCenter.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateGymCenter(GymCenter gymCenter)
        {
            if (ModelState.IsValid)
            {
                _context.GymCenters.Add(gymCenter);
                _context.SaveChanges();
                return RedirectToAction(nameof(GetAllGymCenters));
            }

            return View(gymCenter);
        }

        // 🔹 SALON DÜZENLE (EDIT)
        [HttpGet]
        public IActionResult EditGymCenter(int id)
        {
            var gym = _context.GymCenters.Find(id);
            if (gym == null) return NotFound();

            return View(gym);          // Views/Admin/EditGymCenter.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGymCenter(GymCenter gymCenter)
        {
            if (!ModelState.IsValid)
                return View(gymCenter);

            _context.GymCenters.Update(gymCenter);
            _context.SaveChanges();

            return RedirectToAction(nameof(GetAllGymCenters));
        }

        // 🔹 SALON SİL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteGymCenter(int id)
        {
            var gym = _context.GymCenters
                .Include(g => g.Services)
                .Include(g => g.Trainers)
                .FirstOrDefault(g => g.Id == id);

            if (gym == null) return NotFound();

            if (gym.Services.Any() || gym.Trainers.Any())
            {
                TempData["Error"] = "Bu salona bağlı hizmet veya antrenör olduğu için silinemez.";
                return RedirectToAction(nameof(GetAllGymCenters));
            }

            _context.GymCenters.Remove(gym);
            _context.SaveChanges();

            return RedirectToAction(nameof(GetAllGymCenters));
        }

        // =========================
        // HİZMETLER (SERVICE)
        // =========================

        public IActionResult GetAllServices()
        {
            var services = _context.Services
                .Include(s => s.GymCenter)
                .ToList();

            return View(services);    // Views/Admin/GetAllServices.cshtml
        }

        [HttpGet]
        public IActionResult CreateService()
        {
            var gymCenters = _context.GymCenters.ToList();
            ViewBag.GymCenters = new SelectList(gymCenters, "Id", "Name");
            return View();            // Views/Admin/CreateService.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateService(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Services.Add(service);
                _context.SaveChanges();
                return RedirectToAction(nameof(GetAllServices));
            }

            var gymCenters = _context.GymCenters.ToList();
            ViewBag.GymCenters = new SelectList(gymCenters, "Id", "Name");
            return View(service);
        }

        // 🔹 HİZMET DÜZENLE (EDIT)
        [HttpGet]
        public IActionResult EditService(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();

            var gymCenters = _context.GymCenters.ToList();
            ViewBag.GymCenters = new SelectList(gymCenters, "Id", "Name", service.FitnessCenterId);

            return View(service);     // Views/Admin/EditService.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditService(Service service)
        {
            if (!ModelState.IsValid)
            {
                var gymCenters = _context.GymCenters.ToList();
                ViewBag.GymCenters = new SelectList(gymCenters, "Id", "Name", service.FitnessCenterId);
                return View(service);
            }

            _context.Services.Update(service);
            _context.SaveChanges();

            return RedirectToAction(nameof(GetAllServices));
        }

        // 🔹 HİZMET SİL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteService(int id)
        {
            var service = _context.Services
                .Include(s => s.TrainerServices)
                .Include(s => s.Appointments)
                .FirstOrDefault(s => s.Id == id);

            if (service == null) return NotFound();

            if (service.TrainerServices.Any() || service.Appointments.Any())
            {
                TempData["Error"] = "Bu hizmete bağlı antrenör veya randevu olduğu için silinemez.";
                return RedirectToAction(nameof(GetAllServices));
            }

            _context.Services.Remove(service);
            _context.SaveChanges();

            return RedirectToAction(nameof(GetAllServices));
        }

        // =========================
        // RANDEVU YÖNETİMİ (ADMIN)
        // =========================

        [HttpGet]
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.ApplicationUser)
                .Include(a => a.Trainer)
                    .ThenInclude(t => t.GymCenter)
                .Include(a => a.Trainer)
                    .ThenInclude(t => t.Availabilities)
                .Include(a => a.Service)
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();

            return View(appointments); // Views/Admin/Appointments.cshtml
        }

        // 🔹 SADECE BEKLEYEN RANDEVULAR
        [HttpGet]
        public async Task<IActionResult> PendingAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.ApplicationUser)
                .Include(a => a.Trainer)
                    .ThenInclude(t => t.GymCenter)
                .Include(a => a.Trainer)
                    .ThenInclude(t => t.Availabilities)
                .Include(a => a.Service)
                .Where(a => !a.IsApproved)          // sadece onaylanmamışlar
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            // Aynı görünümü kullanıyoruz, sadece filtrelenmiş liste gönderiyoruz
            return View("Appointments", appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return NotFound();

            appt.IsApproved = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Appointments));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return NotFound();

            _context.Appointments.Remove(appt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Appointments));
        }
    }
}
