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
    // Removing the class-level [Authorize] to allow granular control per method
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ACCESS: Any logged-in user (Faculty, Student, or Admin)
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Branches.AsNoTracking().ToListAsync());
        }

        // ACCESS: Any logged-in user
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var branch = await _context.Branches
                .Include(b => b.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (branch == null) return NotFound();

            return View(branch);
        }

        // ACCESS: Only Administrators
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name,Street,City,Eircode")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(branch.Eircode))
                {
                    branch.Eircode = branch.Eircode.ToUpper().Replace(" ", "");
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

        // ACCESS: Only Administrators
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null) return NotFound();
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Street,City,Eircode")] Branch branch)
        {
            if (id != branch.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchExists(branch.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(branch);
        }

        // ACCESS: Only Administrators
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var branch = await _context.Branches.FirstOrDefaultAsync(m => m.Id == id);
            if (branch == null) return NotFound();
            return View(branch);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasCourses = await _context.Courses.AnyAsync(c => c.BranchId == id);

            if (hasCourses)
            {
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