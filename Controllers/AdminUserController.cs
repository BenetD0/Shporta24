using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shporta24.Data;
using Shporta24.Models;

namespace Shporta24.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminUsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // LIST USERS
        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string password, bool isAdmin)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email dhe Password janë të detyrueshme.");
                return View();
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            // ❌ NËSE DËSHTON
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                // 🔴 AUDIT LOG – FAILED
                _context.AuditLogs.Add(new AuditLog
                {
                    Action = $"FAILED to create user: {email}",
                    PerformedBy = User.Identity?.Name
                });

                await _context.SaveChangesAsync();
                return View();
            }

            // ✅ ROLE
            if (isAdmin)
                await _userManager.AddToRoleAsync(user, "Admin");

            // ✅ AUDIT LOG – SUCCESS
            _context.AuditLogs.Add(new AuditLog
            {
                Action = $"Created user: {email} | Admin: {isAdmin}",
                PerformedBy = User.Identity?.Name
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // EDIT
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, bool isAdmin)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = email;
            user.UserName = email;
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            if (isAdmin && !roles.Contains("Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            if (!isAdmin && roles.Contains("Admin"))
                await _userManager.RemoveFromRoleAsync(user, "Admin");

            // 🔴 AUDIT LOG
            _context.AuditLogs.Add(new AuditLog
            {
                Action = $"Edited user {email} (Admin: {isAdmin})",
                PerformedBy = User.Identity.Name
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);

                // 🔴 AUDIT LOG
                _context.AuditLogs.Add(new AuditLog
                {
                    Action = $"Deleted user {user.Email}",
                    PerformedBy = User.Identity.Name
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // RESET PASSWORD
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(user);
            }

            // 🔴 AUDIT LOG
            _context.AuditLogs.Add(new AuditLog
            {
                Action = $"Reset password for user {user.Email}",
                PerformedBy = User.Identity.Name
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
