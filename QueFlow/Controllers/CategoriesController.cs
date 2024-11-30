using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QueFlow.Data;
using QueFlow.Models;

namespace QueFlow.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        public CategoriesController(
            ApplicationDbContext context
        )
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
