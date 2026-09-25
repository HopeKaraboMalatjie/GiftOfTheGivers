using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAzureFunctionClient _azureFunctionClient;

        public EmployeeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IAzureFunctionClient azureFunctionClient)
        {
            _db = db;
            _userManager = userManager;
            _azureFunctionClient = azureFunctionClient;
        }

        // GET: /Employee/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var projects = await _db.ReliefProjects
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();

            var volunteerSignups = await _db.VolunteerSignups
                .Include(v => v.ReliefProject)
                .OrderByDescending(v => v.SubmittedOn)
                .Take(20)
                .ToListAsync();

            ViewBag.VolunteerSignups = volunteerSignups;
            return View(projects);
        }

        // GET: /Employee/CreateProject
        public IActionResult CreateProject() => View(new ReliefProject());

        // POST: /Employee/CreateProject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(ReliefProject project)
        {
            if (!ModelState.IsValid) return View(project);

            var user = await _userManager.GetUserAsync(User);
            project.PostedByUserId = user?.Id;
            project.StartDate = DateTime.UtcNow;

            _db.ReliefProjects.Add(project);
            await _db.SaveChangesAsync();

            var employeeEmail = user?.Email ?? "unknown employee";
            await _azureFunctionClient.NotifyEmployeeUpdateAsync(project, employeeEmail, HttpContext.RequestAborted);

            return RedirectToAction(nameof(Dashboard));
        }
    }
}
