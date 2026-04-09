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
    public class ExamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exams
        public async Task<IActionResult> Index()
        {
            // Efficiency: AsNoTracking for read-only index and order by date
            var exams = await _context.Exams
                .Include(e => e.Module)
                .AsNoTracking()
                .OrderBy(e => e.ExamDate)
                .ToListAsync();
            return View(exams);
        }

        // GET: Exams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exams = await _context.Exams
                .Include(e => e.Module)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (exams == null) return NotFound();

            return View(exams);
        }

        // GET: Exams/Create
        public IActionResult Create()
        {
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title");
            return View(new Exams { ExamDate = DateTime.Now.AddDays(7) }); // Default to a week from now
        }

        // POST: Exams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModuleId,Title,ExamDate,MaxScore,ResultsReleased")] Exams exams)
        {
            if (ModelState.IsValid)
            {
                // RULE 1: MaxScore Validation
                if (exams.MaxScore <= 0)
                {
                    ModelState.AddModelError("MaxScore", "Max Score must be a positive number.");
                }

                // RULE 2: Logic - Exam date shouldn't be too far in the past
                if (exams.ExamDate < DateTime.Now.AddDays(-1))
                {
                    ModelState.AddModelError("ExamDate", "Exam date cannot be in the past.");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(exams);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title", exams.ModuleId);
            return View(exams);
        }

        // GET: Exams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exams = await _context.Exams.FindAsync(id);
            if (exams == null) return NotFound();

            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title", exams.ModuleId);
            return View(exams);
        }

        // POST: Exams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModuleId,Title,ExamDate,MaxScore,ResultsReleased")] Exams exams)
        {
            if (id != exams.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Re-validate MaxScore
                if (exams.MaxScore <= 0)
                {
                    ModelState.AddModelError("MaxScore", "Max Score must be a positive number.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(exams);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ExamsExists(exams.Id)) return NotFound();
                        else throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title", exams.ModuleId);
            return View(exams);
        }

        // POST: Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // RULE: Referential Integrity - Check for existing results
            bool hasResults = await _context.ExamResults.AnyAsync(er => er.ExamsId == id);

            if (hasResults)
            {
                // Prevent deletion to protect academic records
                return BadRequest("Cannot delete exam because student results are already recorded.");
            }

            var exams = await _context.Exams.FindAsync(id);
            if (exams != null)
            {
                _context.Exams.Remove(exams);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ExamsExists(int id)
        {
            return _context.Exams.Any(e => e.Id == id);
        }
    }
}