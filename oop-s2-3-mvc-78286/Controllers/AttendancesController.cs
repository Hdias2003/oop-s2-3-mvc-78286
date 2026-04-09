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
    public class AttendancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attendances
        public async Task<IActionResult> Index()
        {
            // Efficiency: AsNoTracking and sorting by date
            var attendances = await _context.Attendances
                .Include(a => a.Module)
                .Include(a => a.StudentProfile)
                .AsNoTracking()
                .OrderByDescending(a => a.SessionDate)
                .ToListAsync();
            return View(attendances);
        }

        // GET: Attendances/Create
        public IActionResult Create()
        {
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name");
            return View(new Attendance { SessionDate = DateTime.Now }); // Default to today
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentProfileId,ModuleId,SessionDate,IsPresent")] Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                // RULE: Enforce Granularity (One record per student/module/day)
                bool alreadyExists = await _context.Attendances.AnyAsync(a =>
                    a.StudentProfileId == attendance.StudentProfileId &&
                    a.ModuleId == attendance.ModuleId &&
                    a.SessionDate.Date == attendance.SessionDate.Date);

                if (alreadyExists)
                {
                    ModelState.AddModelError("", "Attendance for this student has already been recorded for this session today.");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(attendance);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title", attendance.ModuleId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", attendance.StudentProfileId);
            return View(attendance);
        }

        // GET: Attendances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null) return NotFound();

            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title", attendance.ModuleId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", attendance.StudentProfileId);
            return View(attendance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentProfileId,ModuleId,SessionDate,IsPresent")] Attendance attendance)
        {
            if (id != attendance.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Verify no date conflict with other records (excluding this record itself)
                bool duplicateConflict = await _context.Attendances.AnyAsync(a =>
                    a.Id != id &&
                    a.StudentProfileId == attendance.StudentProfileId &&
                    a.ModuleId == attendance.ModuleId &&
                    a.SessionDate.Date == attendance.SessionDate.Date);

                if (duplicateConflict)
                {
                    ModelState.AddModelError("", "Changing to this date would create a duplicate attendance record.");
                }
                else
                {
                    try
                    {
                        _context.Update(attendance);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AttendanceExists(attendance.Id)) return NotFound();
                        else throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Title", attendance.ModuleId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", attendance.StudentProfileId);
            return View(attendance);
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.Id == id);
        }
    }
}