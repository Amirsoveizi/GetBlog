using GetBlog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GetBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly GetBlogDbContext _db;

        public HomeController(GetBlogDbContext db)
        {
            this._db = db;
        }

        public async Task<ActionResult> Index()
        {
            var articles = await _db.Articles
                                    .OrderByDescending(a => a.PublishedDate)
                                    .Take(8) 
                                    .ToListAsync();

            return View(articles);
        }

        [ChildActionOnly]
        public ActionResult MainMenu()
        {
            var categories = _db.Categories
                                .Where(c => c.ParentCategoryId == null)
                                .Include(c => c.ChildCategories)
                                .OrderBy(c => c.Name)
                                .ToList();

            return PartialView("_MainMenu", categories);
        }
    }
}