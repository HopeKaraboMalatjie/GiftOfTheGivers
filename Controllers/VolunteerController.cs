using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public VolunteerController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Volunteer/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Projects = await _db.ReliefProjects.Where(p => p.Status == "Active").ToListAsync();
            return View(new VolunteerSignup());
        }

        // POST: /Volunteer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VolunteerSignup signup)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Projects = await _db.ReliefProjects.Where(p => p.Status == "Active").ToListAsync();
                return View(signup);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
            {
                signup.ApplicationUserId = user.Id;
            }

            signup.SubmittedOn = DateTime.UtcNow;

            _db.VolunteerSignups.Add(signup);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Thank you! Your volunteer interest has been recorded.";
            return RedirectToAction(nameof(Confirmation));
        }

        public IActionResult Confirmation() => View();
    }
}
