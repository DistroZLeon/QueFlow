using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize(Roles ="User,Admin")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message=TempData["message"].ToString();
            }
            var categories = from category in db.Categories
                             orderby category.Name
                             select category;
            ViewBag.Categories = categories;
            return View();
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
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
                TempData["message"] = "Categoria a fost adaugata cu succes.";
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
                TempData["message"] = "Categoria a fost modificata cu succes.";
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
            Category categ= db.Categories.Find(id);
            db.Categories.Remove(categ);
            db.SaveChanges();
            TempData["message"] = "Felicitari, ai eliminat o categorie inocenta ce avea o familie iubitoare!";
            return RedirectToAction("Index");
        }
    }
}
