using System;
using System.Linq;
using System.Threading.Tasks;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessCenter.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        private const string AdminEmail = "admin@fitness.com";
        private const string AdminPassword = "Admin123!";

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // ============ REGISTER ============

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                GymCenters = _context.GymCenters
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.GymCenters = _context.GymCenters
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    })
                    .ToList();

                return View(model);
            }

            // admin mailiyle normal kayıt olmasın
            if (string.Equals(model.Email, AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Email", "Bu e-posta sadece yönetici için ayrılmıştır.");

                model.GymCenters = _context.GymCenters
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    })
                    .ToList();

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                GymCenterId = model.GymCenterId   // 🔴 AspNetUsers.GymCenterId
                // FitnessCenterId null kalabilir
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Dashboard", "Musteri");
            }





            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.GymCenters = _context.GymCenters
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                })
                .ToList();

            return View(model);
        }
        // ============ LOGIN ============

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1) Admin girişi (tek admin hesabı)
            if (string.Equals(model.Email, AdminEmail, StringComparison.OrdinalIgnoreCase)
                && model.Password == AdminPassword)
            {
                var adminUser = await _userManager.FindByEmailAsync(AdminEmail);
                if (adminUser == null)
                {
                    // Admin için bir spor salonu ata (ilkini alıyoruz)
                    var firstGymId = _context.GymCenters.Select(g => g.Id).FirstOrDefault();

                    adminUser = new ApplicationUser
                    {
                        UserName = AdminEmail,
                        Email = AdminEmail,
                        FullName = "Admin",
                        GymCenterId = firstGymId
                    };

                    var createResult = await _userManager.CreateAsync(adminUser, AdminPassword);
                    if (!createResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Admin hesabı oluşturulurken bir hata oluştu.");
                        return View(model);
                    }
                }

                await _signInManager.SignInAsync(adminUser, model.RememberMe);
                return RedirectToAction("Index", "Admin");
            }

            // 2) Normal kullanıcı girişi
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Dashboard", "Musteri");
            }




            ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // diğer Login/Logout kısımların aynen kalabilir
    }
}
