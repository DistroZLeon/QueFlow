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
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager
        )
        {
            db = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            ViewBag.UsersList = users;

            return View();

        }
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            ViewBag.UserCurent = await _userManager.GetUserAsync(User);
            return View(user);
        }
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            ViewBag.AllRoles = GetAllRoles();
            var roleNames = await _userManager.GetRolesAsync(user);
            ViewBag.UserRole = _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).Select(r => r.Id).FirstOrDefault();
            return View(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser newuser, [FromForm] string newrole)
        {
            var user = db.Users.Find(id);
            if (newuser.ProfPic != "/images/profile-pictures/default.jpg" && newuser.ProfPic != user.ProfPic) newuser.ProfPic = "/images/profile-pictures/default.jpg";
            if (newuser.Desc != "" && newuser.Desc != user.Desc) newuser.Desc = null;
            user.AllRoles = GetAllRoles();
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage); // Log or debug this line
                    }
                }
            }
            if (ModelState.IsValid)
            {
                user.Alias = newuser.Alias;
                user.Desc = newuser.Desc;
                user.ProfPic = newuser.ProfPic;
                var roles = db.Roles.ToList();
                foreach (var role in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                var roleName = await _roleManager.FindByIdAsync(newrole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.Users.Include("Questions").Include("Answers")
                                .Where(a => a.Id == id).First();
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
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ManageAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var questions = db.Questions.Where(q => q.UserId == user.Id).OrderByDescending(q=>q.Date).Include("User").Include("Category").Take(3);
            var answers = db.Answers.Where(a => a.UserId == user.Id).OrderByDescending(a=>a.DatePosted).Include("User").Take(3);
            ViewBag.Questions = questions;
            ViewBag.answers = answers;
            return View(user);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Manage(ApplicationUser NewUser, IFormFile? ProfilePic)
        {
            var user = await _userManager.GetUserAsync(User);
            if (NewUser.Email == null)
            {
                RedirectToAction("Manage");
            }
            if (ModelState.IsValid)
            {  
                user.Alias = NewUser.Alias;
                user.Nume = NewUser.Nume;
                user.Email = NewUser.Email;
                user.Desc = NewUser.Desc;
                user.NormalizedEmail = NewUser.Email.ToUpper();
                user.UserName = NewUser.Email;
                user.NormalizedUserName = NewUser.Email.ToUpper();
                if (ProfilePic != null && ProfilePic.Length > 0)
                {
                    var fileName = Path.GetFileName(ProfilePic.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profile-pictures", fileName);
                    var directory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePic.CopyToAsync(stream);
                    }
                    user.ProfPic = "/images/profile-pictures/" + fileName;
                }
                else if (ProfilePic == null)
                {
                    user.ProfPic = "/images/profile-pictures/default.jpg";
                }
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);

                    return Redirect("/Home/Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View("Manage", NewUser);
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
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
