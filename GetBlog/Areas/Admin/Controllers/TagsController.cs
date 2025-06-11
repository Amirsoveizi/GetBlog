using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks; 
using System.Web.Mvc;
using GetBlog.Models;

namespace GetBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class TagsController : AdminBaseController
    {
        private readonly GetBlogDbContext _db;

        public TagsController(GetBlogDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<ActionResult> Index()
        {
            var tags = await _db.Tags.OrderBy(t => t.Name).ToListAsync();
            return View(tags);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name, Slug")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                _db.Tags.Add(tag);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tag created successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to create tag. Please check the details.";
            return View(tag);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Tag tag = await _db.Tags.FindAsync(id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TagId, Name, Slug")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(tag).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tag updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to update tag. Please check the details.";
            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            if (await _db.Articles.AnyAsync(a => a.Tags.Any(t => t.TagId == id)))
            {
                TempData["ErrorMessage"] = "Cannot delete this tag because it is currently in use by one or more articles.";
                return RedirectToAction("Index");
            }

            Tag tag = await _db.Tags.FindAsync(id);
            if (tag == null)
            {
                return HttpNotFound();
            }

            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Tag deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}