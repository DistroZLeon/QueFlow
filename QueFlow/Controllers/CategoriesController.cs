using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message=TempData["message"].ToString();
            }
            var categories = from category in db.Categories
                             orderby category.Name
                             select category;
            var search = "";
            ViewBag.SearchString = search;

            int per_page = 4;
            int totalItems=categories.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset =(currentPage-1)*per_page ;
            }
            var paginatedCategories=categories.Skip(offset).Take(per_page);
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)per_page);
            ViewBag.Categories = paginatedCategories;

            if (search!="")
            {
                ViewBag.PageBaseURL = "/Categories/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PageBaseURL = "/Categories/Index/?page";
            }

            return View();
        }
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Include("Questions").Include("Questions.User").Where(a=>a.Id==id).First();

            int per_page = 4;
            int totalItems=category.Questions.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset =(currentPage-1)*per_page ;
            }

            var pagedQuestions=category.Questions.Skip(offset).Take(per_page).ToList();
            category.Questions = pagedQuestions;
            ViewBag.lastPage= Math.Ceiling((float)totalItems / (float)pagedQuestions.Count());
            ViewBag.PageBaseURL = "/Categories/Show/" + id + "?page";
            return View(category);
        }
        [Authorize(Roles ="Admin")]
        public ActionResult New()
        {
            return View();
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult New(Category cat)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(cat);
                db.SaveChanges();
                TempData["message"] = "The category has been added";
                return RedirectToAction("Index");
            }
            else
            {
                return View(cat);
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id) 
        { 
            Category cat= db.Categories.Find(id);
            return View(cat);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(int id, Category nou)
        {
            Category category = db.Categories.Find(id);
            if (ModelState.IsValid)
            {
                category.Name = nou.Name;
                category.Description = nou.Description;
                db.SaveChanges();
                TempData["message"] = "The category has been modified";
                return RedirectToAction("Index");
            }
            else
            {
                return View(nou);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Category categ= db.Categories.Include("Questions")
                                         .Where(que => que.Id == id)
                                         .First();
            foreach (var que in categ.Questions)
            {
                Question question = db.Questions.Include("Answers")
                                            .Where(q => q.Id == que.Id)
                                            .First();
                foreach (var ans in question.Answers) db.Answers.Remove(ans);
                db.Questions.Remove(que);
            }
            db.Categories.Remove(categ);
            db.SaveChanges();
            TempData["message"] = "The category has been deleted";
            return RedirectToAction("Index");
        }
    }
}
