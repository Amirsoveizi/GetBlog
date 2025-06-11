using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using GetBlog.Models;

namespace GetBlog.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly GetBlogDbContext _db;

        public ArticlesController(GetBlogDbContext dbContext)
        {
            _db = dbContext;
        }

        [Route("Articles/Category/{categorySlug}")]
        public async Task<ActionResult> Category(string categorySlug)
        {
            var category = await _db.Categories
                                    .FirstOrDefaultAsync(c => c.Slug == categorySlug);

            if (category == null)
            {
                return HttpNotFound();
            }

            var articlesInCategory = await _db.Articles
                                              .Where(a => a.IsPublished && a.Categories.Any(c => c.CategoryId == category.CategoryId))
                                              .OrderByDescending(a => a.PublishedDate)
                                              .ToListAsync();

            ViewBag.CategoryName = category.Name;

            return View(articlesInCategory);
        }

        public async Task<ActionResult> Details(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Query by ArticleId instead of Slug
            var article = await _db.Articles
                                   .Include(a => a.Author.Profile)
                                   .Include(a => a.Categories)
                                   .Include(a => a.Tags)
                                   .Include(a => a.Comments.Select(c => c.Author.Profile))
                                   .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null)
            {
                return HttpNotFound();
            }

            var currentArticleCategoryIds = article.Categories.Select(c => c.CategoryId).ToList();
            var relatedArticles = await _db.Articles
                                           .Where(a => a.IsPublished && a.ArticleId != article.ArticleId &&
                                                       a.Categories.Any(c => currentArticleCategoryIds.Contains(c.CategoryId)))
                                           .OrderByDescending(a => a.PublishedDate)
                                           .Take(2)
                                           .ToListAsync();

            ViewBag.ExcellentRelatedArticles = relatedArticles;

            return View(article);
        }
    }
}