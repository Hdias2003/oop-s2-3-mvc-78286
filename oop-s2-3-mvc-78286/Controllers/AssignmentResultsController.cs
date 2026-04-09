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
    public class AssignmentResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssignmentResults
        public async Task<IActionResult> Index()
        {
            // Efficiency: Added AsNoTracking and ordering for easier management
            var results = await _context.AssignmentResults
                .Include(a => a.Assignment)
                .Include(a => a.StudentProfile)
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .ToListAsync();
            return View(results);
        }

        // POST: AssignmentResults/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssignmentId,StudentProfileId,Result")] AssignmentResults assignmentResults)
        {
            if (ModelState.IsValid)
            {
                // Fetch assignment details for validation
                var assignment = await _context.Assignments.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == assignmentResults.AssignmentId);

                // RULE 1: Max Score Validation
                if (assignment != null && assignmentResults.Result > (decimal)assignment.MaxScore)
                {
                    ModelState.AddModelError("Result", $"Score {assignmentResults.Result} exceeds the maximum of {assignment.MaxScore}.");
                }

                // RULE 2: Unique Result (Student + Assignment combo)
                bool alreadyExists = await _context.AssignmentResults.AnyAsync(ar =>
                    ar.AssignmentId == assignmentResults.AssignmentId &&
                    ar.StudentProfileId == assignmentResults.StudentProfileId);

                if (alreadyExists)
                {
                    ModelState.AddModelError("", "A result for this student on this assignment already exists.");
                }

                if (ModelState.IsValid) // Re-check after custom rules
                {
                    _context.Add(assignmentResults);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            PopulateDropdowns(assignmentResults);
            return View(assignmentResults);
        }

        // POST: AssignmentResults/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssignmentId,StudentProfileId,Result")] AssignmentResults assignmentResults)
        {
            if (id != assignmentResults.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Re-validate MaxScore on Edit
                var assignment = await _context.Assignments.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == assignmentResults.AssignmentId);

                if (assignment != null && assignmentResults.Result > (decimal)assignment.MaxScore)
                {
                    ModelState.AddModelError("Result", $"Score {assignmentResults.Result} exceeds the maximum of {assignment.MaxScore}.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(assignmentResults);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AssignmentResultsExists(assignmentResults.Id)) return NotFound();
                        else throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            PopulateDropdowns(assignmentResults);
            return View(assignmentResults);
        }

        // Private helper to keep code DRY (Don't Repeat Yourself)
        private void PopulateDropdowns(AssignmentResults result = null)
        {
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", result?.AssignmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", result?.StudentProfileId);
        }

        private bool AssignmentResultsExists(int id)
        {
            return _context.AssignmentResults.Any(e => e.Id == id);
        }
    }
}