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
    public class StaffProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StaffProfiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StaffProfiles.Include(s => s.Role).Include(s => s.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StaffProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffProfile = await _context.StaffProfiles
                .Include(s => s.Role)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staffProfile == null)
            {
                return NotFound();
            }

            return View(staffProfile);
        }

        // GET: StaffProfiles/Create
        public IActionResult Create()
        {
            ViewData["RoleName"] = new SelectList(_context.Roles, "Id", "Id");
            ViewData["IdentityUserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: StaffProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoleName,IdentityUserID,Name")] StaffProfile staffProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staffProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleName"] = new SelectList(_context.Roles, "Id", "Id", staffProfile.RoleName);
            ViewData["IdentityUserID"] = new SelectList(_context.Users, "Id", "Id", staffProfile.IdentityUserID);
            return View(staffProfile);
        }

        // GET: StaffProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffProfile = await _context.StaffProfiles.FindAsync(id);
            if (staffProfile == null)
            {
                return NotFound();
            }
            ViewData["RoleName"] = new SelectList(_context.Roles, "Id", "Id", staffProfile.RoleName);
            ViewData["IdentityUserID"] = new SelectList(_context.Users, "Id", "Id", staffProfile.IdentityUserID);
            return View(staffProfile);
        }

        // POST: StaffProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoleName,IdentityUserID,Name")] StaffProfile staffProfile)
        {
            if (id != staffProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staffProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffProfileExists(staffProfile.Id))
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
            ViewData["RoleName"] = new SelectList(_context.Roles, "Id", "Id", staffProfile.RoleName);
            ViewData["IdentityUserID"] = new SelectList(_context.Users, "Id", "Id", staffProfile.IdentityUserID);
            return View(staffProfile);
        }

        // GET: StaffProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffProfile = await _context.StaffProfiles
                .Include(s => s.Role)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staffProfile == null)
            {
                return NotFound();
            }

            return View(staffProfile);
        }

        // POST: StaffProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staffProfile = await _context.StaffProfiles.FindAsync(id);
            if (staffProfile != null)
            {
                _context.StaffProfiles.Remove(staffProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffProfileExists(int id)
        {
            return _context.StaffProfiles.Any(e => e.Id == id);
        }
    }
}
