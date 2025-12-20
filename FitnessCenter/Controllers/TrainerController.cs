using FitnessCenter.Data;
using FitnessCenter.Models;
using FitnessCenter.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

//[Authorize(Roles = "Admin")]
public class TrainerController : Controller
{
    private readonly ApplicationDbContext _context;

    public TrainerController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Trainer/Create
    [HttpGet]
    public IActionResult Create()
    {
        var vm = new TrainerCreateViewModel
        {
            GymCenters = _context.GymCenters.ToList(),
            Services = _context.Services.ToList()
        };

        // Varsayılan saatler
        vm.AvailableFrom = DateTime.Now;
        vm.AvailableTo = DateTime.Now.AddHours(1);

        return View(vm);
    }

    // POST: /Trainer/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainerCreateViewModel model)
    {
        Console.WriteLine(">>> Create POST çalıştı prenses!");
        // 1) Basit validasyon
        if (model.AvailableTo <= model.AvailableFrom)
        {
            ModelState.AddModelError("", "Bitiş zamanı başlangıçtan sonra olmalıdır.");
        }

        if (!ModelState.IsValid)
        {
            // Hata olduğunda dropdown’lar boş kalmasın diye tekrar doldur
            model.GymCenters = _context.GymCenters.ToList();
            model.Services = _context.Services.ToList();
            return View(model);
        }

        // 2) Antrenörü oluştur
        var trainer = new Trainer
        {
            Name = model.Name,
            FitnessCenterId = model.FitnessCenterId
        };

        _context.Trainers.Add(trainer);
        await _context.SaveChangesAsync(); // Burası ÇOK kritik!

        // 3) Seçili servislerden TrainerService oluştur
        if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
        {
            foreach (var serviceId in model.SelectedServiceIds)
            {
                _context.TrainerServices.Add(new TrainerService
                {
                    TrainerId = trainer.Id,
                    ServiceId = serviceId
                });
            }
        }

        // 4) Müsaitlik kaydı
        var availability = new TrainerAvailability
        {
            TrainerId = trainer.Id,
            AvailableFrom = model.AvailableFrom,
            AvailableTo = model.AvailableTo
        };

        _context.TrainerAvailabilities.Add(availability);

        // 5) Son kez kaydet
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Test için basit Index
    public IActionResult Index()
    {
        var trainers = _context.Trainers
            .Include(t => t.GymCenter)
            .Include(t => t.TrainerServices).ThenInclude(ts => ts.Service)
            .ToList();

        return View(trainers);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var trainer = await _context.Trainers
            .Include(t => t.TrainerServices)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trainer == null)
            return NotFound();

        var availability = await _context.TrainerAvailabilities
            .FirstOrDefaultAsync(a => a.TrainerId == id);

        var vm = new TrainerCreateViewModel
        {
            Name = trainer.Name,
            FitnessCenterId = trainer.FitnessCenterId,
            SelectedServiceIds = trainer.TrainerServices?
                .Select(ts => ts.ServiceId)
                .ToArray(),

            AvailableFrom = availability?.AvailableFrom ?? DateTime.Now,
            AvailableTo = availability?.AvailableTo ?? DateTime.Now.AddHours(1),

            GymCenters = _context.GymCenters.ToList(),
            Services = _context.Services.ToList()
        };

        ViewBag.TrainerId = id;
        return View(vm);
    }

    // =======================
    // EDIT (POST)
    // =======================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TrainerCreateViewModel model)
    {
        if (model.AvailableTo <= model.AvailableFrom)
        {
            ModelState.AddModelError("", "Bitiş zamanı başlangıçtan sonra olmalıdır.");
        }

        if (!ModelState.IsValid)
        {
            model.GymCenters = _context.GymCenters.ToList();
            model.Services = _context.Services.ToList();
            ViewBag.TrainerId = id;
            return View(model);
        }

        var trainer = await _context.Trainers
            .Include(t => t.TrainerServices)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trainer == null)
            return NotFound();

        // Temel bilgiler
        trainer.Name = model.Name;
        trainer.FitnessCenterId = model.FitnessCenterId;
        trainer.SpecialtyIds ??= ""; // garanti olsun

        // Servis ilişkilerini güncelle
        var existingTrainerServices = _context.TrainerServices
            .Where(ts => ts.TrainerId == id);
        _context.TrainerServices.RemoveRange(existingTrainerServices);

        if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
        {
            foreach (var sid in model.SelectedServiceIds)
            {
                _context.TrainerServices.Add(new TrainerService
                {
                    TrainerId = id,
                    ServiceId = sid
                });
            }
        }

        // Müsaitlik güncelle
        var availability = await _context.TrainerAvailabilities
            .FirstOrDefaultAsync(a => a.TrainerId == id);

        if (availability == null)
        {
            availability = new TrainerAvailability
            {
                TrainerId = id,
                AvailableFrom = model.AvailableFrom,
                AvailableTo = model.AvailableTo
            };
            _context.TrainerAvailabilities.Add(availability);
        }
        else
        {
            availability.AvailableFrom = model.AvailableFrom;
            availability.AvailableTo = model.AvailableTo;
        }

        await _context.SaveChangesAsync();

        // İstersen Admin paneline dön:
        // return RedirectToAction("Index", "Admin");

        // Şimdilik antrenör listesine dönelim:
        return RedirectToAction(nameof(Index));
    }

    // =======================
    // DELETE (GET)
    // =======================
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var trainer = await _context.Trainers
            .Include(t => t.GymCenter)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trainer == null)
            return NotFound();

        return View(trainer);
    }

    // =======================
    // DELETE (POST)
    // =======================
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var trainer = await _context.Trainers
            .Include(t => t.TrainerServices)
            .Include(t => t.Availabilities)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trainer == null)
            return NotFound();

        // Önce ilişkileri sil
        if (trainer.TrainerServices != null && trainer.TrainerServices.Any())
            _context.TrainerServices.RemoveRange(trainer.TrainerServices);

        if (trainer.Availabilities != null && trainer.Availabilities.Any())
            _context.TrainerAvailabilities.RemoveRange(trainer.Availabilities);

        // Sonra antrenörü sil
        _context.Trainers.Remove(trainer);

        await _context.SaveChangesAsync();

        // İstersen Admin paneline de dönebilirsin:
        // return RedirectToAction("Index", "Admin");

        return RedirectToAction(nameof(Index));
    }

}
