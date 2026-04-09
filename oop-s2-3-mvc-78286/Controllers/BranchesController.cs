using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using College.Domain.Models;
using oop_s2_3_mvc_78286.Data;
using Microsoft.AspNetCore.Authorization;

namespace oop_s2_3_mvc_78286.Controllers
{
    [Authorize(Roles = "Administrator")] // Restrict branch management to Admins
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
            // Efficiency: AsNoTracking for read-only campus list
            return View(await _context.Branches.AsNoTracking().ToListAsync());
        }

        // GET: Branches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Logic Suggestion: Include courses so we can see what's taught at this branch
            var branch = await _context.Branches
                .Include(b => b.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (branch == null) return NotFound();

            return View(branch);
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Street,City,Eircode")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                // RULE: Simple Eircode formatting (Ensure uppercase and remove extra spaces)
                if (!string.IsNullOrEmpty(branch.Eircode))
                {
                    branch.Eircode = branch.Eircode.ToUpper().Replace(" ", "");

                    // Basic length check for Irish Eircodes (7 characters)
                    if (branch.Eircode.Length != 7)
                    {
                        ModelState.AddModelError("Eircode", "A valid Eircode must be exactly 7 characters.");
                    }
                }

                if (ModelState.IsValid)
                {
                    _context.Add(branch);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // RULE: Referential Integrity - Don't delete a branch if it has courses
            bool hasCourses = await _context.Courses.AnyAsync(c => c.BranchId == id);

            if (hasCourses)
            {
                // In a production app, use TempData to show a Toast/Alert to the user
                ModelState.AddModelError("", "Cannot delete branch. Move or delete existing courses first.");

                var branch = await _context.Branches.FindAsync(id);
                return View("Delete", branch);
            }

            var branchToDelete = await _context.Branches.FindAsync(id);
            if (branchToDelete != null)
            {
                _context.Branches.Remove(branchToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BranchExists(int id)
        {
            return _context.Branches.Any(e => e.Id == id);
        }
    }
}