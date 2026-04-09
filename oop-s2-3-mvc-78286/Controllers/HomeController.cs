using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_3_mvc_78286.Data;
using oop_s2_3_mvc_78286.Models;

namespace oop_s2_3_mvc_78286.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Gathering statistics for the dashboard cards
            // Using AsNoTracking() for performance since this is read-only
            ViewBag.BranchCount = await _context.Branches.AsNoTracking().CountAsync();
            ViewBag.StudentCount = await _context.StudentProfiles.AsNoTracking().CountAsync();
            ViewBag.CourseCount = await _context.Courses.AsNoTracking().CountAsync();

            // Getting the 5 most recent exam results to show "Recent Activity"
            var recentResults = await _context.ExamResults
                .Include(er => er.StudentProfile)
                .Include(er => er.Exams)
                .OrderByDescending(er => er.Id)
                .Take(5)
                .AsNoTracking()
                .ToListAsync();

            return View(recentResults);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}