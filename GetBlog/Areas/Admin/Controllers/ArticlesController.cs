using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GetBlog.Models;
using Microsoft.AspNet.Identity;      
using Microsoft.AspNet.Identity.Owin; 
namespace GetBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Editor, Author")]
    public class ArticlesController : AdminBaseController
    {
        private readonly GetBlogDbContext _db;
        private ApplicationUserManager _userManager;

        public ArticlesController(GetBlogDbContext dbContext)
        {
            _db = dbContext;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public async Task<ActionResult> Index()
        {
            var articles = await _db.Articles
                                    .Include(a => a.Author)
                                    .Include(a => a.Categories)
                                    .OrderByDescending(a => a.CreatedDate)
                                    .ToListAsync();
            return View(articles);
        }

        private async Task PopulateDropdowns(Article article = null)
        {
            var allCategories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
            var selectedCategoryIds = article?.Categories.Select(c => c.CategoryId).ToArray();
            ViewBag.CategoriesList = new MultiSelectList(allCategories, "CategoryId", "Name", selectedCategoryIds);

            var allTags = await _db.Tags.OrderBy(t => t.Name).ToListAsync();
            var selectedTagIds = article?.Tags.Select(t => t.TagId).ToArray();
            ViewBag.TagsList = new MultiSelectList(allTags, "TagId", "Name", selectedTagIds);

            var allAuthors = await UserManager.Users.OrderBy(u => u.FullName).ToListAsync();
            ViewBag.AuthorId = new SelectList(allAuthors, "Id", "FullName", article?.AuthorId);
        }

        public async Task<ActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new Article());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(
    [Bind(Include = "Title, Content, Excerpt, Slug, IsPublished")] Article article,
    HttpPostedFileBase imageUpload, 
    int[] categoryIds,
    int[] tagIds,
    string AuthorId)
        {
            if (imageUpload != null && imageUpload.ContentLength > 0)
            {
                if (!imageUpload.ContentType.Contains("image"))
                {
                    ModelState.AddModelError("imageUpload", "The uploaded file must be an image.");
                }
                else
                {
                    var uploadPath = Server.MapPath("~/images/"); 
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageUpload.FileName).ToLower();
                    var fullPath = Path.Combine(uploadPath, fileName);
                    imageUpload.SaveAs(fullPath);

                    article.FeaturedImageUrl = "/images/" + fileName;
                }
            }


            if (!User.IsInRole("Admin") && !User.IsInRole("Editor"))
            {
                article.AuthorId = User.Identity.GetUserId();
            }
            else
            {
                article.AuthorId = AuthorId;
            }

            if (ModelState.IsValid)
            {
                article.CreatedDate = DateTime.UtcNow;
                article.LastModifiedDate = DateTime.UtcNow;
                if (article.IsPublished)
                {
                    article.PublishedDate = DateTime.UtcNow;
                }

                if (categoryIds != null)
                {
                    article.Categories = await _db.Categories.Where(c => categoryIds.Contains(c.CategoryId)).ToListAsync();
                }
                if (tagIds != null)
                {
                    article.Tags = await _db.Tags.Where(t => tagIds.Contains(t.TagId)).ToListAsync();
                }

                _db.Articles.Add(article);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Article created successfully!";
                return RedirectToAction("Index");
            }

            await PopulateDropdowns(article);
            return View(article);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = await _db.Articles.Include(a => a.Categories).Include(a => a.Tags).FirstOrDefaultAsync(a => a.ArticleId == id);
            if (article == null)
            {
                return HttpNotFound();
            }

            if (!User.IsInRole("Admin") && !User.IsInRole("Editor") && article.AuthorId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not authorized to edit this article.");
            }

            await PopulateDropdowns(article);
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(int id, int[] categoryIds, int[] tagIds)
        {
            var articleToUpdate = await _db.Articles.Include(a => a.Categories).Include(a => a.Tags).FirstOrDefaultAsync(a => a.ArticleId == id);

            if (articleToUpdate == null) return HttpNotFound();
            if (!User.IsInRole("Admin") && !User.IsInRole("Editor") && articleToUpdate.AuthorId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (TryUpdateModel(articleToUpdate, "", new string[] { "Title", "Content", "Excerpt", "FeaturedImageUrl", "Slug", "IsPublished", "AuthorId" }))
            {
                articleToUpdate.LastModifiedDate = DateTime.UtcNow;
                if (articleToUpdate.IsPublished && articleToUpdate.PublishedDate == null)
                {
                    articleToUpdate.PublishedDate = DateTime.UtcNow;
                }

                articleToUpdate.Categories.Clear();
                if (categoryIds != null)
                {
                    articleToUpdate.Categories = await _db.Categories.Where(c => categoryIds.Contains(c.CategoryId)).ToListAsync();
                }

                articleToUpdate.Tags.Clear();
                if (tagIds != null)
                {
                    articleToUpdate.Tags = await _db.Tags.Where(t => tagIds.Contains(t.TagId)).ToListAsync();
                }

                _db.Entry(articleToUpdate).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Article updated successfully!";
                return RedirectToAction("Index");
            }

            await PopulateDropdowns(articleToUpdate);
            return View(articleToUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Editor")]
        public async Task<ActionResult> Delete(int id)
        {
            Article article = await _db.Articles.FindAsync(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            _db.Articles.Remove(article);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Article deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}