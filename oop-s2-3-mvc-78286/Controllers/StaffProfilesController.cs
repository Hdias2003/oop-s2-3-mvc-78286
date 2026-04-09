using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using College.Domain.Models;
using oop_s2_3_mvc_78286.Data;
using Microsoft.AspNetCore.Authorization;

namespace oop_s2_3_mvc_78286.Controllers
{
    [Authorize(Roles = "Administrator")] // Only Admins should manage staff profiles
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
            // Efficiency: Include related entities and use AsNoTracking
            var staff = await _context.StaffProfiles
                .Include(s => s.Role)
                .Include(s => s.User)
                .AsNoTracking()
                .ToListAsync();
            return View(staff);
        }

        // GET: StaffProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var staffProfile = await _context.StaffProfiles
                .Include(s => s.Role)
                .Include(s => s.User)
                .Include(s => s.Modules) // Include Many-to-Many modules taught
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (staffProfile == null) return NotFound();

            return View(staffProfile);
        }

        // GET: StaffProfiles/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoleName,IdentityUserID,Name")] StaffProfile staffProfile)
        {
            if (ModelState.IsValid)
            {
                // RULE: 1-to-1 Relationship Check
                // Prevent creating a second profile for the same Identity User
                bool userHasProfile = await _context.StaffProfiles.AnyAsync(s => s.IdentityUserID == staffProfile.IdentityUserID);

                if (userHasProfile)
                {
                    ModelState.AddModelError("IdentityUserID", "This login user is already linked to a staff profile.");
                }
                else
                {
                    _context.Add(staffProfile);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            PopulateDropdowns(staffProfile);
            return View(staffProfile);
        }

        // POST: StaffProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // RULE: Integrity Check
            // Check if this staff member is currently assigned to any modules
            var staffProfile = await _context.StaffProfiles
                .Include(s => s.Modules)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (staffProfile != null)
            {
                if (staffProfile.Modules.Any())
                {
                    return BadRequest("Cannot delete staff profile. They are currently assigned to teach one or more modules.");
                }

                _context.StaffProfiles.Remove(staffProfile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper to improve readability and fix dropdown labels
        private void PopulateDropdowns(StaffProfile staff = null)
        {
            // Show Role Name and User Email/Username instead of IDs
            ViewData["RoleName"] = new SelectList(_context.Roles, "Id", "Name", staff?.RoleName);
            ViewData["IdentityUserID"] = new SelectList(_context.Users, "Id", "Email", staff?.IdentityUserID);
        }

        private bool StaffProfileExists(int id)
        {
            return _context.StaffProfiles.Any(e => e.Id == id);
        }
    }
}