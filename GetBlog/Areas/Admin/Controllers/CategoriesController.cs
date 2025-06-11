using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks; 
using System.Web.Mvc;
using GetBlog.Models;

namespace GetBlog.Areas.Admin.Controllers
{
    public class CategoriesController : AdminBaseController
    {
        private readonly GetBlogDbContext _db;

        public CategoriesController(GetBlogDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<ActionResult> Index()
        {
            var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
            return View(categories);
        }

        private async Task PopulateParentCategoryDropdown(int? excludeId = null, object selectedValue = null)
        {
            var categoriesQuery = _db.Categories.OrderBy(c => c.Name).AsQueryable();
            if (excludeId != null)
            {
                categoriesQuery = categoriesQuery.Where(c => c.CategoryId != excludeId);
            }
            ViewBag.ParentCategoryId = new SelectList(await categoriesQuery.ToListAsync(), "CategoryId", "Name", selectedValue);
        }

        public async Task<ActionResult> Create()
        {
            await PopulateParentCategoryDropdown();
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name, Slug, ParentCategoryId")] Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to create category. Please check the details and try again.";
            await PopulateParentCategoryDropdown(selectedValue: category.ParentCategoryId);
            return View(category);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            await PopulateParentCategoryDropdown(excludeId: id, selectedValue: category.ParentCategoryId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CategoryId, Name, Slug, ParentCategoryId")] Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(category).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to update category. Please check the details and try again.";
            await PopulateParentCategoryDropdown(excludeId: category.CategoryId, selectedValue: category.ParentCategoryId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            if (await _db.Articles.AnyAsync(a => a.Categories.Any(c => c.CategoryId == id)))
            {
                TempData["ErrorMessage"] = "Cannot delete this category because it is currently in use by one or more articles.";
                return RedirectToAction("Index");
            }

            Category category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Category deleted successfully!";

            return RedirectToAction("Index");
        }
    }
}