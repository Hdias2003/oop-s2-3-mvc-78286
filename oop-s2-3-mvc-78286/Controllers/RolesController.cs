using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using College.Domain.Models;
using oop_s2_3_mvc_78286.Data;
using Microsoft.AspNetCore.Authorization;

namespace oop_s2_3_mvc_78286.Controllers
{
    // ONLY Administrators should ever see this controller
    [Authorize(Roles = "Administrator")]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            // Efficiency: NoTracking for a simple list
            return View(await _context.Roles.AsNoTracking().ToListAsync());
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Role role)
        {
            if (ModelState.IsValid)
            {
                // RULE 1: Enforce Unique Role Names (Case-Insensitive)
                bool exists = await _context.Roles.AnyAsync(r => r.Name.ToUpper() == role.Name.ToUpper());
                if (exists)
                {
                    ModelState.AddModelError("Name", "This role already exists.");
                }
                else
                {
                    // RULE 2: Manually handle NormalizedName for Identity compatibility
                    role.Id = Guid.NewGuid().ToString();
                    role.NormalizedName = role.Name.ToUpper();
                    role.ConcurrencyStamp = Guid.NewGuid().ToString();

                    _context.Add(role);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // RULE 3: Strict Referential Integrity
            // Prevent deletion if any User is currently assigned to this Role
            // (Assuming IdentityUserRole table exists)
            bool isAssigned = await _context.UserRoles.AnyAsync(ur => ur.RoleId == id);

            if (isAssigned)
            {
                return BadRequest("Cannot delete role. There are users currently assigned to this role.");
            }

            // RULE 4: Protect System-Critical Roles
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                if (role.Name == "Administrator")
                {
                    return BadRequest("The 'Administrator' role is protected and cannot be deleted.");
                }

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(string id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}