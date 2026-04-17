using BlindMatchPAS.Data;
using BlindMatchPAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // My Projects List
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var projects = await _context.Projects
                .Include(p => p.ResearchArea)
                .Where(p => p.StudentId == user!.Id)
                .ToListAsync();
            return View(projects);
        }

        // Submit New Project
        public async Task<IActionResult> Create()
        {
            ViewBag.ResearchAreas = await _context.ResearchAreas.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project project, IFormFile? uploadedFile)
        {
            var user = await _userManager.GetUserAsync(User);
            project.StudentId = user!.Id;
            project.Status = "Pending";

            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() +
                    Path.GetExtension(uploadedFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }
                project.FilePath = "/uploads/" + fileName;
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // View Status
        public async Task<IActionResult> Status(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ResearchArea)
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.Id == id);
            return View(project);
        }
    }
}