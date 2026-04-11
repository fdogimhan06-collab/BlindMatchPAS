using BlindMatchPAS.Data;
using BlindMatchPAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Controllers
{
    [Authorize(Roles = "ModuleLeader")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // All Matches Dashboard
        public async Task<IActionResult> Index()
        {
            var matches = await _context.Matches
                .Include(m => m.Project)
                .ThenInclude(p => p!.Student)
                .Include(m => m.Supervisor)
                .ToListAsync();
            return View(matches);
        }

        // Manage Research Areas
        public async Task<IActionResult> ResearchAreas()
        {
            var areas = await _context.ResearchAreas.ToListAsync();
            return View(areas);
        }

        [HttpPost]
        public async Task<IActionResult> AddResearchArea(string name)
        {
            _context.ResearchAreas.Add(new ResearchArea { Name = name });
            await _context.SaveChangesAsync();
            return RedirectToAction("ResearchAreas");
        }

        // All Users
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
    }
}