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
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Assignments
        public async Task<IActionResult> Index()
        {
            // Efficiency Suggestion: Using AsNoTracking() for read-only lists
            var assignments = await _context.Assignments
                .Include(a => a.Module)
                .AsNoTracking()
                .ToListAsync();
            return View(assignments);
        }

        // GET: Assignments/Create
        public IActionResult Create()
        {
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModuleId,Title,MaxScore,StartDate,DueDate,Visibility")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                // RULE 1: Chronological Date Validation
                if (assignment.DueDate <= assignment.StartDate)
                {
                    ModelState.AddModelError("DueDate", "The Due Date must be after the Start Date.");
                }

                // RULE 2: Minimum MaxScore
                if (assignment.MaxScore <= 0)
                {
                    ModelState.AddModelError("MaxScore", "Max Score must be a positive value.");
                }

                if (ModelState.ErrorCount == 0)
                {
                    _context.Add(assignment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title", assignment.ModuleId);
            return View(assignment);
        }

        // GET: Assignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title", assignment.ModuleId);
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModuleId,Title,MaxScore,StartDate,DueDate,Visibility")] Assignment assignment)
        {
            if (id != assignment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Re-apply logical checks on Edit
                if (assignment.DueDate <= assignment.StartDate)
                {
                    ModelState.AddModelError("DueDate", "The Due Date must be after the Start Date.");
                }

                if (ModelState.ErrorCount == 0)
                {
                    try
                    {
                        _context.Update(assignment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AssignmentExists(assignment.Id)) return NotFound();
                        else throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title", assignment.ModuleId);
            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // RULE: Prevent deletion if results are already recorded
            bool hasResults = await _context.AssignmentResults.AnyAsync(ar => ar.AssignmentId == id);

            if (hasResults)
            {
                // In a real app, you'd pass this error to the Delete View via TempData
                return BadRequest("Cannot delete assignment because student results are already recorded.");
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}