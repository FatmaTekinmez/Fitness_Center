using FitnessCenter.Data;
using FitnessCenter.Models;
using FitnessCenter.Models.ViewModels;
using FitnessCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessCenter.Controllers
{
    [Authorize] // sadece giriş yapanlar
    public class MusteriController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly GeminiService _geminiService;

        public MusteriController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            GeminiService geminiService)
        {
            _userManager = userManager;
            _context = context;
            _geminiService = geminiService;
        }

        // =======================
        // KULLANICI DASHBOARD
        // =======================
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.Users
                .Include(u => u.GymCenter)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login", "Account");

            double? bmi = null;
            string bmiCategory = null;

            if (user.HeightCm.HasValue && user.WeightKg.HasValue)
            {
                double heightM = user.HeightCm.Value / 100.0;
                bmi = user.WeightKg.Value / (heightM * heightM);

                if (bmi < 18.5) bmiCategory = "Zayıf";
                else if (bmi < 25) bmiCategory = "Normal";
                else if (bmi < 30) bmiCategory = "Fazla Kilolu";
                else bmiCategory = "Obez";
            }

            var vm = new MusteriDashboardViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                HeightCm = user.HeightCm,
                WeightKg = user.WeightKg,
                BMI = bmi,
                BmiCategory = bmiCategory,
                GymCenterName = user.GymCenter?.Name,
                GymCenterId = user.GymCenterId
            };

            return View(vm);
        }

        // =======================
        // SPOR SALONLARI LİSTESİ
        // =======================
        [HttpGet]
        public async Task<IActionResult> GymCenters()
        {
            var gyms = await _context.GymCenters.ToListAsync();
            return View(gyms); // Views/Musteri/GymCenters.cshtml
        }

        // =======================
        // RANDEVU AL (GET)
        // =======================
        [HttpGet]
        public async Task<IActionResult> BookAppointment(int gymCenterId)
        {
            var gym = await _context.GymCenters
                .Include(g => g.Trainers)
                    .ThenInclude(t => t.TrainerServices)
                        .ThenInclude(ts => ts.Service)
                .FirstOrDefaultAsync(g => g.Id == gymCenterId);

            if (gym == null)
                return NotFound();

            var vm = new AppointmentCreateViewModel
            {
                GymCenterId = gymCenterId,
                GymCenterName = gym.Name,
                Trainers = gym.Trainers,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            return View(vm); // Views/Musteri/BookAppointment.cshtml
        }

        // =======================
        // RANDEVU AL (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(AppointmentCreateViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var gym = await _context.GymCenters
                .Include(g => g.Trainers)
                .FirstOrDefaultAsync(g => g.Id == model.GymCenterId);

            if (gym == null)
                return NotFound();

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("", "Bitiş zamanı başlangıçtan sonra olmalıdır.");
            }

            // Seçilen antrenör gerçekten bu salona ait mi?
            var trainer = await _context.Trainers
                .Include(t => t.Availabilities)
                .Include(t => t.TrainerServices)
                .FirstOrDefaultAsync(t =>
                    t.Id == model.TrainerId &&
                    t.FitnessCenterId == model.GymCenterId);

            if (trainer == null)
            {
                ModelState.AddModelError("", "Geçersiz antrenör seçimi.");
            }

            // Seçilen servis, bu antrenöre bağlı mı?
            TrainerService trainerService = null;
            Service service = null;

            if (trainer != null)
            {
                trainerService = trainer.TrainerServices
                    .FirstOrDefault(ts => ts.ServiceId == model.ServiceId);

                if (trainerService == null)
                {
                    ModelState.AddModelError("", "Seçilen hizmet, bu antrenöre ait değil.");
                }
                else
                {
                    service = await _context.Services
                        .FirstOrDefaultAsync(s => s.Id == model.ServiceId);
                }

                // Müsaitlik kontrolü
                var fitsAvailability = trainer.Availabilities.Any(a =>
                    model.StartTime >= a.AvailableFrom &&
                    model.EndTime <= a.AvailableTo
                );

                if (!fitsAvailability)
                {
                    ModelState.AddModelError("", "Seçilen zaman, antrenörün müsaitlik saatleri içinde değil.");
                }

                // Çakışan randevu kontrolü
                var hasConflict = await _context.Appointments.AnyAsync(a =>
                    a.TrainerId == trainer.Id &&
                    model.StartTime < a.EndTime &&
                    model.EndTime > a.StartTime
                );

                if (hasConflict)
                {
                    ModelState.AddModelError("", "Bu saat aralığında antrenörün başka bir randevusu var.");
                }
            }

            if (!ModelState.IsValid)
            {
                model.GymCenterName = gym.Name;
                model.Trainers = await _context.Trainers
                    .Include(t => t.TrainerServices)
                        .ThenInclude(ts => ts.Service)
                    .Where(t => t.FitnessCenterId == model.GymCenterId)
                    .ToListAsync();

                return View(model);
            }

            var appointment = new Appointment
            {
                ApplicationUserId = user.Id,
                TrainerId = trainer!.Id,
                ServiceId = model.ServiceId,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                IsApproved = false,
                PriceAtBooking = service?.Price ?? 0m
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }

        // =======================
        // KULLANICININ RANDEVULARI
        // =======================
        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var appointments = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Where(a => a.ApplicationUserId == user.Id)
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();

            return View(appointments); // Views/Musteri/MyAppointments.cshtml
        }

        // =======================
        // RANDEVU İPTAL (MÜŞTERİ)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.ApplicationUserId == user.Id);

            if (appointment == null)
            {
                return NotFound();
            }

            // İstersen geçmiş randevuların iptal edilmesini engelleyebilirsin:
            if (appointment.StartTime <= DateTime.Now)
            {
                TempData["Error"] = "Başlamış veya geçmiş randevuları iptal edemezsiniz.";
                return RedirectToAction("MyAppointments");
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Randevunuz başarıyla iptal edildi.";
            return RedirectToAction("MyAppointments");
        }

        // =======================
        // AI ÖNERİ SAYFASI
        // =======================
        [HttpGet]
        public IActionResult AiRecommendation()
        {
            return View(new AiRecommendationViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AiRecommendation(AiRecommendationViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Goal) &&
                string.IsNullOrWhiteSpace(model.Level) &&
                string.IsNullOrWhiteSpace(model.Preferences))
            {
                ModelState.AddModelError(string.Empty, "Lütfen en az bir alanı doldurun.");
                return View(model);
            }

            var result = await _geminiService.GetWorkoutRecommendationAsync(
                model.Goal,
                model.Level,
                model.Preferences
            );

            model.Recommendation = result;
            return View(model);
        }

        // =======================
        // BOY-KİLO GÜNCELLEME
        // =======================
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
    }
}
