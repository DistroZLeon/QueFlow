using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser newuser, [FromForm] string newrole)
        {
            var user= db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            if (ModelState.IsValid)
            {
                user.Desc = newuser.Desc;
                user.ProfPic= newuser.ProfPic;
                var roles = db.Roles.ToList();
                foreach (var role in roles)
                {
                    await userManager.RemoveFromRoleAsync(user, role.Name);                
                }
                var roleName=await roleManager.FindByIdAsync(newrole);
                await userManager.AddToRoleAsync(user,roleName.ToString());
                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(string id) 
        {
            var user = db.Users.Include("Questions").Include("Answers")
                                .Where(a=>a.Id==id).First();
            if (user.Answers.Count() > 0)
            {
                foreach (var answer in user.Answers)
                {
                    db.Answers.Remove(answer);
                }
            }
            if (user.Questions.Count() > 0)
            {
                foreach (var question in user.Questions)
                {
                    foreach(var ans in question.Answers)
                    {
                        db.Answers.Remove(ans);
                    }
                    db.Questions.Remove(question);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList= new List<SelectListItem>();
            var roles= from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}
