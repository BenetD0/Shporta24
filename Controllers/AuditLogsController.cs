using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shporta24.Data;
using Shporta24.Models;
using System.Linq;

namespace Shporta24.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var logs = _context.AuditLogs
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(logs);
        }
    }
}
