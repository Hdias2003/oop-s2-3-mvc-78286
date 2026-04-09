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
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
            // Efficiency: Include related collections to show counts in the view
            return View(await _context.Modules
                .Include(m => m.Courses)
                .Include(m => m.StaffProfiles)
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @module = await _context.Modules
                .Include(m => m.Courses)
                .Include(m => m.StaffProfiles)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@module == null) return NotFound();

            return View(@module);
        }

        // GET: Modules/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] Module @module)
        {
            if (ModelState.IsValid)
            {
                // Logic: Ensure module titles are unique to prevent confusion
                if (await _context.Modules.AnyAsync(m => m.Title == @module.Title))
                {
                    ModelState.AddModelError("Title", "A module with this title already exists.");
                }
                else
                {
                    _context.Add(@module);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(@module);
        }

        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @module = await _context.Modules.FindAsync(id);
            if (@module == null) return NotFound();

            return View(@module);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] Module @module)
        {
            if (id != @module.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // RULE: Referential Integrity - Check dependencies
            bool hasAssignments = await _context.Assignments.AnyAsync(a => a.ModuleId == id);
            bool hasAttendance = await _context.Attendances.AnyAsync(a => a.ModuleId == id);

            if (hasAssignments || hasAttendance)
            {
                return BadRequest("Cannot delete module. It has associated assignments or attendance records.");
            }

            var @module = await _context.Modules.FindAsync(id);
            if (@module != null)
            {
                _context.Modules.Remove(@module);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }
    }
}