using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
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
            SetAccessRights();
            var questions = db.Questions.Include("Category").Include("User");
            ViewBag.Questions = questions;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"].ToString();
            }
            return View();
        }
        public IActionResult Show(int id)
        {
            SetAccessRights();
            Question question = db.Questions.Include("Category").Include("Answers").Include("User")
                              .Where(a => a.Id == id)
                              .First();

            ViewBag.Question = question;

            ViewBag.Category = question.Category;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"].ToString();
            }
            return View(question);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Show([FromForm] Answer answer)
        {
            answer.DatePosted = DateTime.Now;
            answer.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return Redirect("/Questions/Show/" + answer.QuestionId);
            }
            else
            {
                Question que = db.Questions.Include("Category").Include("Answers").Where(que => que.Id == answer.QuestionId).First();
                return View(que);
            }
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Question Question = new Question();
            Question.Categ = GetAllCategories();
            return View(Question);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Question question)
        {
            question.Date = DateTime.Now;
            question.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                TempData["message"] = "Intrebarea a fost publicata cu success";
                return RedirectToAction("Index");
            }
            else
            {
                question.Categ = GetAllCategories();
                return View(question);
            }
        }
        public IActionResult Edit(int id)
        {
            Question  question = db.Questions.Include("Category")
                                         .Where(que => que.Id == id)
                                         .First();

            question.Categ = GetAllCategories();
            if (question.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                return View(question);
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificati intrebarea altui utilizator";
                return Redirect("/Questions/Show/" + question.Id.ToString());
            }

        }
        [HttpPost]
        public IActionResult Edit(Question question)
        {
            Question que = db.Questions.Find(question.Id);
            if (que.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                if (ModelState.IsValid)
                {
                    Question intrebare = db.Questions.Find(question.Id);
                    intrebare.Title = question.Title;
                    intrebare.Content = question.Content;
                    intrebare.CategoryId = question.CategoryId;
                    db.SaveChanges();
                    TempData["message"] = "Intrebarea a fost modificata";
                    return RedirectToAction("Index");

                }
                else
                {
                    question.Categ = GetAllCategories();
                    return View(question);
                }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificati intrebarea altui utilizator";
                return RedirectToAction("Index");
            }

        }
        public IActionResult EditCat(int id)
        {
            Question question = db.Questions.Include("Category")
                                         .Where(que => que.Id == id)
                                         .First();

            question.Categ = GetAllCategories();
            if (question.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                return View(question);
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificati intrebarea altui utilizator";
                return Redirect("/Questions/Show/" + question.Id.ToString());
            }

        }
        [HttpPost]
        public IActionResult EditCat(Question question)
        {
            if (User.IsInRole("Admin"))
                if (question.CategoryId!=null)
                {
                    Question intrebare = db.Questions.Find(question.Id);
                    intrebare.CategoryId = question.CategoryId;
                    db.SaveChanges();
                    TempData["message"] = "Intrebarea a fost modificata";
                    return RedirectToAction("Index");

                }
                else
                {
                    question.Categ = GetAllCategories();
                    return View(question);
                }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificati intrebarea altui utilizator";
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Question question = db.Questions.Include("Answers")
                                            .Where(que => que.Id == id)
                                            .First();
            if (question.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                foreach (var ans in question.Answers) db.Answers.Remove(ans);
                db.Questions.Remove(question);
                db.SaveChanges();
                TempData["message"] = "Intrebarea a fost stearsa";

                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti intrebarea altui utilizator";
                return Redirect("/Questions/Show/" + question.Id.ToString());
            }

        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();
            var categories = from cat in db.Categories
                             select cat;
            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                });
            }
            return selectList;
        }
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;
            if (User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoare = true;
            }
            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}