using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FitnessCenter.Controllers
{
    [Authorize] // sadece giriş yapanlar
    public class MusteriController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public MusteriController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // KULLANICI PANELİ
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.Users
                .Include(u => u.GymCenter)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login", "Account");

            double? bmi = null;

            if (user.HeightCm.HasValue && user.WeightKg.HasValue)
            {
                double heightM = user.HeightCm.Value / 100.0;
                bmi = user.WeightKg.Value / (heightM * heightM);
            }

            var vm = new MusteriDashboardViewModel
            {
                FullName = user.FullName,
                HeightCm = user.HeightCm,
                WeightKg = user.WeightKg,
                BMI = bmi,
                GymCenterName = user.GymCenter?.Name
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateMetrics(MusteriDashboardViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            user.HeightCm = model.HeightCm;
            user.WeightKg = model.WeightKg;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        // Randevu alma sayfasını sonra burada açarsın
        public IActionResult BookAppointment()
        {
            // Spor salonu, hizmet, eğitmen vs seçimi burada olacak
            return View();
        }
    }
}
