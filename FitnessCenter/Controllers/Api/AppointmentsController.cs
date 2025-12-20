using FitnessCenter.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessCenter.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/appointments/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var data = await _context.Appointments
                .Where(a => !a.IsApproved)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Include(a => a.ApplicationUser)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    a.Id,
                    MemberName = a.ApplicationUser.FullName,
                    TrainerName = a.Trainer.Name,
                    ServiceName = a.Service.Name,
                    a.StartTime,
                    a.EndTime,
                    a.PriceAtBooking
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
