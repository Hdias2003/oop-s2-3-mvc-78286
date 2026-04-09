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
    public class ExamResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExamResults
        public async Task<IActionResult> Index()
        {
            // Efficiency: AsNoTracking and order by the most recent results
            var results = await _context.ExamResults
                .Include(e => e.Exams)
                .Include(e => e.StudentProfile)
                .AsNoTracking()
                .OrderByDescending(e => e.Id)
                .ToListAsync();
            return View(results);
        }

        // POST: ExamResults/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentProfileId,ExamsId,Score,Status")] ExamResults examResults)
        {
            if (ModelState.IsValid)
            {
                // RULE 1: Fetch the Exam to check against MaxScore (if Exams model has a MaxScore property)
                var exam = await _context.Set<Exams>().AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == examResults.ExamsId);

                // Note: If your Exams model has a MaxScore, enforce it here:
                // if (exam != null && examResults.Score > exam.MaxScore) { ... }

                // RULE 2: Prevent duplicate results for the same exam
                bool alreadyExists = await _context.ExamResults.AnyAsync(er =>
                    er.StudentProfileId == examResults.StudentProfileId &&
                    er.ExamsId == examResults.ExamsId);

                if (alreadyExists)
                {
                    ModelState.AddModelError("", "A result for this student on this specific exam has already been recorded.");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(examResults);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            PopulateDropdowns(examResults);
            return View(examResults);
        }

        // POST: ExamResults/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentProfileId,ExamsId,Score,Status")] ExamResults examResults)
        {
            if (id != examResults.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Check for duplicate conflicts (excluding current record)
                bool duplicateConflict = await _context.ExamResults.AnyAsync(er =>
                    er.Id != id &&
                    er.StudentProfileId == examResults.StudentProfileId &&
                    er.ExamsId == examResults.ExamsId);

                if (duplicateConflict)
                {
                    ModelState.AddModelError("", "Cannot save: This student already has a result for this exam.");
                }
                else
                {
                    try
                    {
                        _context.Update(examResults);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ExamResultsExists(examResults.Id)) return NotFound();
                        else throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            PopulateDropdowns(examResults);
            return View(examResults);
        }

        private void PopulateDropdowns(ExamResults result = null)
        {
            ViewData["ExamsId"] = new SelectList(_context.Set<Exams>(), "Id", "Title", result?.ExamsId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", result?.StudentProfileId);
        }

        private bool ExamResultsExists(int id)
        {
            return _context.ExamResults.Any(e => e.Id == id);
        }
    }
}