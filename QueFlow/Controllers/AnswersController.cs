using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QueFlow.Data;
using QueFlow.Models;

namespace QueFlow.Controllers
{
    public class AnswersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AnswersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Answer ans = db.Answers.Find(id);
            if (ans.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin")){
                db.Answers.Remove(ans);
                db.SaveChanges();
                TempData["message"] = "The answer has been deleted";
                return Redirect("/Questions/Show/" + ans.QuestionId);
            }
            else
            {
                TempData["message"] = "You do not have permission to delete another user's answer";
                return Redirect("/Questions/Show/"+ans.QuestionId);
            }
        }
        public IActionResult Edit(int id)
        {
            Answer ans=db.Answers.Find(id);
            if(ans.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                ViewBag.Comment = ans;
                return View(ans);
            }
            else
            {
                TempData["message"] = "You do not have permission to edit another user's answer";
                return Redirect("/Questions/Show/" + ans.QuestionId);
            }
        }
        [HttpPost]
        public IActionResult Edit(int id, Answer nou)
        {
            Answer ans= db.Answers.Find(id);
            if(ans.UserId== _userManager.GetUserId(User)||User.IsInRole("Admin"))
            { 
                if (ModelState.IsValid)
                {
                    ans.Text = nou.Text;
                    db.SaveChanges();
                    TempData["message"] = "The answer has been edited";
                    return Redirect("/Questions/Show/" + ans.QuestionId);
                }
                else
                {
                    return View(nou);
                }
            }
            else
            {
                TempData["message"] = "You do not have permission to edit another user's answer";
                return Redirect("/Questions/Show/" + ans.QuestionId);
            }
        }
    }
}
