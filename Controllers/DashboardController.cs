using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shporta24.Data;

namespace Shporta24.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.TotalUsers = _context.Users.Count();

            return View();
        }
    }
}
