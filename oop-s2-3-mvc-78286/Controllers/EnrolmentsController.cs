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
    public class EnrolmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Enrolments
        public async Task<IActionResult> Index()
        {
            // Efficiency: Using AsNoTracking and eager loading for performance
            var enrolments = await _context.Enrolments
                .Include(e => e.Course)
                .Include(e => e.StudentProfile)
                .AsNoTracking()
                .ToListAsync();
            return View(enrolments);
        }

        // POST: Enrolments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentProfileId,CourseId,StartDate,EndDate,Status")] Enrolments enrolments)
        {
            if (ModelState.IsValid)
            {
                // RULE 1: Fetch course to validate dates
                var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == enrolments.CourseId);

                if (course != null)
                {
                    if (enrolments.StartDate < course.StartDate || enrolments.EndDate > course.EndDate)
                    {
                        ModelState.AddModelError("StartDate", $"Enrolment dates must fall within the Course dates: {course.StartDate.ToShortDateString()} - {course.EndDate.ToShortDateString()}");
                    }
                }

                // RULE 2: Prevent duplicate active enrolments for the same student/course
                bool isAlreadyEnrolled = await _context.Enrolments.AnyAsync(e =>
                    e.StudentProfileId == enrolments.StudentProfileId &&
                    e.CourseId == enrolments.CourseId &&
                    e.Status == "Active");

                if (isAlreadyEnrolled)
                {
                    ModelState.AddModelError("", "This student is already actively enrolled in this course.");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(enrolments);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            PopulateDropdowns(enrolments);
            return View(enrolments);
        }

        // POST: Enrolments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentProfileId,CourseId,StartDate,EndDate,Status")] Enrolments enrolments)
        {
            if (id != enrolments.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Re-validate dates on Edit
                var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == enrolments.CourseId);
                if (course != null && (enrolments.StartDate < course.StartDate || enrolments.EndDate > course.EndDate))
                {
                    ModelState.AddModelError("StartDate", "Dates must remain within the course duration.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(enrolments);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EnrolmentsExists(enrolments.Id)) return NotFound();
                        else throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            PopulateDropdowns(enrolments);
            return View(enrolments);
        }

        // Helper to keep dropdown logic consistent
        private void PopulateDropdowns(Enrolments enrolment = null)
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", enrolment?.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", enrolment?.StudentProfileId);
        }

        private bool EnrolmentsExists(int id)
        {
            return _context.Enrolments.Any(e => e.Id == id);
        }
    }
}