using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QueFlow.Data;
using QueFlow.Models;

namespace QueFlow.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public QuestionsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }  
        public IActionResult Index()
        {
            return View();
        }
    }
}
