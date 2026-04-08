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
            var applicationDbContext = _context.AssignmentResults.Include(a => a.Assignment).Include(a => a.StudentProfile);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AssignmentResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResults = await _context.AssignmentResults
                .Include(a => a.Assignment)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignmentResults == null)
            {
                return NotFound();
            }

            return View(assignmentResults);
        }

        // GET: AssignmentResults/Create
        public IActionResult Create()
        {
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "IdentityUserID");
            return View();
        }

        // POST: AssignmentResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssignmentId,StudentProfileId,Result")] AssignmentResults assignmentResults)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignmentResults);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResults.AssignmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "IdentityUserID", assignmentResults.StudentProfileId);
            return View(assignmentResults);
        }

        // GET: AssignmentResults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResults = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResults == null)
            {
                return NotFound();
            }
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResults.AssignmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "IdentityUserID", assignmentResults.StudentProfileId);
            return View(assignmentResults);
        }

        // POST: AssignmentResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssignmentId,StudentProfileId,Result")] AssignmentResults assignmentResults)
        {
            if (id != assignmentResults.Id)
            {
                return NotFound();
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
                    if (!AssignmentResultsExists(assignmentResults.Id))
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
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResults.AssignmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "IdentityUserID", assignmentResults.StudentProfileId);
            return View(assignmentResults);
        }

        // GET: AssignmentResults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResults = await _context.AssignmentResults
                .Include(a => a.Assignment)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignmentResults == null)
            {
                return NotFound();
            }

            return View(assignmentResults);
        }

        // POST: AssignmentResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignmentResults = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResults != null)
            {
                _context.AssignmentResults.Remove(assignmentResults);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentResultsExists(int id)
        {
            return _context.AssignmentResults.Any(e => e.Id == id);
        }
    }
}
