using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QueFlow.Data;
using QueFlow.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QueFlow.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
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
            var users = from user in db.Users
                       orderby user.UserName
                       select user;
            ViewBag.UsersList = users;

            return View();

        }

        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user=db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            ViewBag.UserCurent = await _userManager.GetUserAsync(User);
            return View(user);
        }
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user= db.Users.Find(id);
            ViewBag.AllRoles = GetAllRoles();
            var roleNames = await _userManager.GetRolesAsync(user);
            ViewBag.UserRole= _roleManager.Roles.Where(r=>roleNames.Contains(r.Name)).Select(r=>r.Id).FirstOrDefault();
            return View(user);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser newuser, [FromForm] string newrole)
        {
            var user= db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            if(ModelState.IsValid)
            {
                user.UserName= newuser.UserName;
                user.Desc = newuser.Desc;
                user.ProfPic= newuser.ProfPic;
                var roles = db.Roles.ToList();
                foreach (var role in roles) 
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);                
                }
                var roleName=await _roleManager.FindByIdAsync(newrole);
                await _userManager.AddToRoleAsync(user,roleName.ToString());
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
                foreach (var que in user.Questions)
                {
                    Question question = db.Questions.Include("Answers")
                                                .Where(q => q.Id == que.Id)
                                                .First();
                    foreach (var ans in question.Answers) db.Answers.Remove(ans);
                    db.Questions.Remove(que);
                }
            }
            db.Users.Remove(user);
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
