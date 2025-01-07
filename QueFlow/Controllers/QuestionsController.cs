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
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"].ToString();
            }
            var search = "";
            var filter = "";
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                List<int> articleIds = db.Questions.Where
                                        (
                                         at => at.Title.Contains(search)
                                         || at.Content.Contains(search)
                                        ).Select(a => a.Id).ToList();
                List<int> articleIdsOfCommentsWithSearchString = db.Answers
                                        .Where
                                        (
                                         c => c.Text.Contains(search)
                                        ).Select(c => (int)c.QuestionId).ToList();
                List<int> mergedIds = articleIds.Union(articleIdsOfCommentsWithSearchString).ToList();
                if (Convert.ToString(HttpContext.Request.Query["filter"]) == null) {
                    questions = db.Questions.Where(article => mergedIds.Contains(article.Id))
                                          .Include("Category")
                                          .Include("User")
                                          .OrderByDescending(a => a.Date);
                }
                else
                {
                    filter = Convert.ToString(HttpContext.Request.Query["filter"]);
                    if (filter == "1")
                    {
                        questions = db.Questions.Where(article => mergedIds.Contains(article.Id))
                                          .Include("Category")
                                          .Include("User")
                                          .OrderBy(a => a.Date);
                    }
                    if (filter == "2")
                    {
                        questions = db.Questions.Where(article => mergedIds.Contains(article.Id))
                                          .Include("Category")
                                          .Include("User")
                                          .OrderByDescending(a => a.Date);
                    }
                    if (filter == "3")
                    {
                        questions = db.Questions.Where(article => mergedIds.Contains(article.Id))
                                          .Include("Category")
                                          .Include("User")
                                          .OrderBy(a => a.Answers.Count);
                    }
                    if(filter == "4")
                    {
                        questions = db.Questions.Where(article => mergedIds.Contains(article.Id))
                                          .Include("Category")
                                          .Include("User")
                                          .OrderByDescending(a => a.Answers.Count);
                    }
                }

            }
            else if(Convert.ToString(HttpContext.Request.Query["filter"]) != null)
            {
                filter = Convert.ToString(HttpContext.Request.Query["filter"]);
                if (filter == "1")
                {
                    questions = db.Questions.Include("Category").Include("User")
                                      .OrderBy(a => a.Date);
                }
                if (filter == "2")
                {
                    questions = db.Questions.Include("Category").Include("User")
                                      .OrderByDescending(a => a.Date);
                }
                if (filter == "3")
                {
                    questions = db.Questions.Include("Category").Include("User")
                                      .OrderBy(a => a.Answers.Count);
                }
                if (filter == "4")
                {
                    questions = db.Questions.Include("Category").Include("User")
                                      .OrderByDescending(a => a.Answers.Count);
                }
            }
            ViewBag.SearchString = search;
            int _perPage = 5;
            int totalItems = questions.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedQuestions = questions.Skip(offset).Take(_perPage);
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Questions = paginatedQuestions;
            if (search != "")
            {
                if (filter != "")
                {
                    ViewBag.PaginationBaseUrl = "/Questions/Index/?search=" + search + "&filter=" + filter + "&page";
                    ViewBag.PaginationFilterUrl = "/Questions/Index/?search=" + search + "&filter";
                }
                else
                {
                    ViewBag.PaginationBaseUrl = "/Questions/Index/?search=" + search + "&page";
                    ViewBag.PaginationFilterUrl = "/Questions/Index/?search=" + search + "&filter";
                }
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Questions/Index/?page";
                ViewBag.PaginationFilterUrl = "/Questions/Index/?filter";
            }
            ViewBag.Filter = filter;
            return View();
        }
        public IActionResult Show(int id)
        {
            SetAccessRights();
            Question question = db.Questions.Include("Category").Include("Answers.User").Include("User")
                              .Where(a => a.Id == id)
                              .First();
            Console.WriteLine(question.Answers);
            ViewBag.Question = question;
            var filter = "";
            ViewBag.Answers = question.Answers.ToList();
            if (Convert.ToString(HttpContext.Request.Query["filter"]) != null)
            {
                filter = Convert.ToString(HttpContext.Request.Query["filter"]);
                if (filter == "1")
                {
                    ViewBag.Answers = question.Answers.OrderBy(a => a.DatePosted).ToList();
                }
                if (filter == "2")
                {
                    ViewBag.Answers = question.Answers.OrderByDescending(a => a.DatePosted).ToList();
                }
                if (filter == "3")
                {
                    ViewBag.Answers = question.Answers.OrderBy(a => a.Text).ToList();
                }
                if (filter == "4")
                {
                    ViewBag.Answers = question.Answers.OrderByDescending(a => a.Text).ToList();
                }
            }
            ViewBag.FilterUrl = "/Questions/Show/" + id.ToString() + "?filter";
            ViewBag.Filter = filter;
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
                TempData["message"] = "The question has been posted";
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
                TempData["message"] = "You do not have permission to edit another user's question";
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
                    TempData["message"] = "The question has been modified";
                    return RedirectToAction("Index");

                }
                else
                {
                    question.Categ = GetAllCategories();
                    return View(question);
                }
            else
            {
                TempData["message"] = "You do not have permission to edit another user's question";
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
                TempData["message"] = "You do not have permission to edit another user's question";
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
                    TempData["message"] = "The question has been modified";
                    return RedirectToAction("Index");

                }
                else
                {
                    question.Categ = GetAllCategories();
                    return View(question);
                }
            else
            {
                TempData["message"] = "You do not have permission to edit another user's question";
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
                TempData["message"] = "The question has been deleted";

                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You do not have permission to delete another user's question";
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