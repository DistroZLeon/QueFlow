using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QueFlow.Data;
using QueFlow.Models;

namespace QueFlow.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> uManager,
            RoleManager<IdentityRole> rManager
        )
        {
            db = context;
            userManager = uManager;
            roleManager = rManager; 
        }
        public IActionResult Index()
        {
            var users = from user in db.Users
                       orderby user.UserName
                       select user;
            ViewBag.UsersList = users;

            return View();

        }

        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user=db.Users.Find(id);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            ViewBag.UserCurent = await userManager.GetUserAsync(User);
            return View(user);
        }
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user= db.Users.Find(id);
            ViewBag.AllRoles = roleManager.Roles;
            var roleNames = await userManager.GetRolesAsync(user);
            ViewBag.UserRole= roleManager.Roles
                .Where(r=>roleNames.Contains(r.Name)).Select(r=>r.Id).First();
            return View(user);
        }
        public async Task<ActionResult> Edit(string id, ApplicationUser newuser, [FromForm] string newrole)
        {
            var user= db.Users.Find(id);
            //user.AllRoles = GetAllRoles();
            return View(user);
        }
    }
}
