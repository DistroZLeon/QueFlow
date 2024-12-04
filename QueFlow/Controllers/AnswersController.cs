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
                return Redirect("/Questions/Show/" + ans.QuestionId);
            }
            else
            {
                TempData["message"] = "Nu aveti permisiunile necesare pentru a sterge acest raspuns! (Esti asa de sensibil incat simti nevoia sa stergi raspunsul cuiva?)";
                return Redirect("/Questions/Show"+ans.QuestionId);
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
                TempData["message"] = "Nu aveti permisiunile necesare sa editati acest raspuns!";
                return Redirect("/Questions/Show" + ans.QuestionId);
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
                    return Redirect("/Questions/Show" + ans.QuestionId);
                }
                else
                {
                    return View(nou);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti permisiunile necesare sa editati acest raspuns! (Te crezi Dumnezeu sa moderezi raspunsurile altora?)";
                return Redirect("/Questions/Show" + ans.QuestionId);
            }
        }
    }
}
