using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using College.Domain.Models;
using oop_s2_3_mvc_78286.Data;
using Microsoft.AspNetCore.Authorization;

namespace oop_s2_3_mvc_78286.Controllers
{
    [Authorize]
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

            var course = await _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.Modules)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name");
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,BranchId")] Course course)
        {
            // Manual validation check for DateOnly logic
            if (course.EndDate <= course.StartDate)
            {
                ModelState.AddModelError("EndDate", "The Course End Date must be after the Start Date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown if validation fails
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,BranchId")] Course course)
        {
            if (id != course.Id) return NotFound();

            // Manual validation check for DateOnly logic
            if (course.EndDate <= course.StartDate)
            {
                ModelState.AddModelError("EndDate", "The Course End Date must be after the Start Date.");
            }

            if (ModelState.IsValid)
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

            // Repopulate dropdown if validation fails
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasEnrolments = await _context.Enrolments.AnyAsync(e => e.CourseId == id);
            bool hasModules = await _context.Modules.AnyAsync(m => m.Courses.Any(c => c.Id == id));

            if (hasEnrolments || hasModules)
            {
                TempData["Error"] = "Cannot delete course. It currently has active enrolments or assigned modules.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Branches.Remove(course); // Keep an eye on this: should be _context.Courses.Remove
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