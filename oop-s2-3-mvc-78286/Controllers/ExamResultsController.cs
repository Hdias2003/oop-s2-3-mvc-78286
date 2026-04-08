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
            var applicationDbContext = _context.ExamResults.Include(e => e.Exams).Include(e => e.StudentProfile);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ExamResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResults = await _context.ExamResults
                .Include(e => e.Exams)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examResults == null)
            {
                return NotFound();
            }

            return View(examResults);
        }

        // GET: ExamResults/Create
        public IActionResult Create()
        {
            ViewData["ExamsId"] = new SelectList(_context.Set<Exams>(), "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "IdentityUserID");
            return View();
        }

        // POST: ExamResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentProfileId,ExamsId,Score,Status")] ExamResults examResults)
        {
            if (ModelState.IsValid)
            {
                _context.Add(examResults);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExamsId"] = new SelectList(_context.Set<Exams>(), "Id", "Title", examResults.ExamsId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "IdentityUserID", examResults.StudentProfileId);
            return View(examResults);
        }

        // GET: ExamResults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResults = await _context.ExamResults.FindAsync(id);
            if (examResults == null)
            {
                return NotFound();
            }
            ViewData["ExamsId"] = new SelectList(_context.Set<Exams>(), "Id", "Title", examResults.ExamsId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "IdentityUserID", examResults.StudentProfileId);
            return View(examResults);
        }

        // POST: ExamResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentProfileId,ExamsId,Score,Status")] ExamResults examResults)
        {
            if (id != examResults.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examResults);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamResultsExists(examResults.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExamsId"] = new SelectList(_context.Set<Exams>(), "Id", "Title", examResults.ExamsId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "IdentityUserID", examResults.StudentProfileId);
            return View(examResults);
        }

        // GET: ExamResults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResults = await _context.ExamResults
                .Include(e => e.Exams)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examResults == null)
            {
                return NotFound();
            }

            return View(examResults);
        }

        // POST: ExamResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examResults = await _context.ExamResults.FindAsync(id);
            if (examResults != null)
            {
                _context.ExamResults.Remove(examResults);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamResultsExists(int id)
        {
            return _context.ExamResults.Any(e => e.Id == id);
        }
    }
}
