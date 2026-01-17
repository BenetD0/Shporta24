using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shporta24.Data;
using Shporta24.Models;
using System.Diagnostics;

namespace Shporta24.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= Index (Home Page) =================
        // Parametra opcional: categoryId + search
        public async Task<IActionResult> Index(int? categoryId, string search)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Filter per kategori
            if (categoryId.HasValue)
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);

            // Filter per emrin e produktit (search)
            if (!string.IsNullOrWhiteSpace(search))
                productsQuery = productsQuery.Where(p => p.Name.Contains(search));

            // Merr te gjitha kategorite per sidebar
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SearchText = search;

            var products = await productsQuery.ToListAsync();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
