using BlindMatchPAS.Data;
using BlindMatchPAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupervisorController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Blind Review Dashboard
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.ResearchArea)
                .Where(p => p.Status == "Pending" || p.Status == "UnderReview")
                .ToListAsync();
            return View(projects);
        }

        // Express Interest
        public async Task<IActionResult> ExpressInterest(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            // Check if already expressed interest
            var existing = await _context.Matches
                .FirstOrDefaultAsync(m => m.ProjectId == id
                    && m.SupervisorId == user!.Id);

            if (existing == null)
            {
                var match = new Match
                {
                    ProjectId = id,
                    SupervisorId = user!.Id,
                    IsRevealed = false
                };
                _context.Matches.Add(match);

                var project = await _context.Projects.FindAsync(id);
                project!.Status = "UnderReview";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyMatches");
        }

        // Confirm Match - Identity Reveal
        public async Task<IActionResult> ConfirmMatch(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Project)
                .ThenInclude(p => p!.Student)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            match!.IsRevealed = true;
            match.Project!.Status = "Matched";
            await _context.SaveChangesAsync();
            return View(match);
        }

        // My Matches - Under Review
        public async Task<IActionResult> MyMatches()
        {
            var user = await _userManager.GetUserAsync(User);
            var matches = await _context.Matches
                .Include(m => m.Project)
                .ThenInclude(p => p!.ResearchArea)
                .Where(m => m.SupervisorId == user!.Id)
                .ToListAsync();
            return View(matches);
        }

        // View Project Details
        public async Task<IActionResult> ProjectDetails(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ResearchArea)
                .FirstOrDefaultAsync(p => p.Id == id);
            return View(project);
        }
    }
}