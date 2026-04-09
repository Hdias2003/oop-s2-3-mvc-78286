using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using College.Domain.Models;
using oop_s2_3_mvc_78286.Data;

namespace oop_s2_3_mvc_78286.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            // Efficiency: Include Branch and use AsNoTracking for read-only lists
            var courses = await _context.Courses
                .Include(c => c.Branch)
                .AsNoTracking()
                .ToListAsync();
            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Include Modules to see the curriculum in the details view
            var course = await _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.Modules)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,BranchId")] Course course)
        {
            if (ModelState.IsValid)
            {
                // RULE 1: Chronological Validation
                if (course.EndDate <= course.StartDate)
                {
                    ModelState.AddModelError("EndDate", "The Course End Date must be after the Start Date.");
                }

                if (ModelState.ErrorCount == 0)
                {
                    _context.Add(course);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,BranchId")] Course course)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Re-validate dates on Edit
                if (course.EndDate <= course.StartDate)
                {
                    ModelState.AddModelError("EndDate", "The Course End Date must be after the Start Date.");
                }

                if (ModelState.ErrorCount == 0)
                {
                    try
                    {
                        _context.Update(course);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CourseExists(course.Id)) return NotFound();
                        else throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // RULE: Referential Integrity - Check for Enrolments or Modules
            bool hasEnrolments = await _context.Enrolments.AnyAsync(e => e.CourseId == id);
            bool hasModules = await _context.Modules.AnyAsync(m => m.Courses.Any(c => c.Id == id));

            if (hasEnrolments || hasModules)
            {
                // Prevent deletion if active links exist
                return BadRequest("Cannot delete course. It currently has active enrolments or assigned modules.");
            }

            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}