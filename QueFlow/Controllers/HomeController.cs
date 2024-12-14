using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueFlow.Data;
using QueFlow.Models;
using System.Diagnostics;

namespace QueFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;
        public HomeController(ApplicationDbContext context,ILogger<HomeController> logger)
        {
            _logger = logger;
            db = context;
        }

        public IActionResult Index()
        {
            var categories = db.Categories.Where(c => c.Questions.Any()).Select(c => new Category
            {
                Id = c.Id,
                Name=c.Name,
                Questions = c.Questions.OrderByDescending(q =>q.Date).Take(5).ToList()
            }
            ).Take(3).ToList();
            ViewBag.Categories=categories;
            
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
