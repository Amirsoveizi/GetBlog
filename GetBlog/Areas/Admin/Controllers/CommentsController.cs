using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks; 
using System.Web.Mvc;
using GetBlog.Models;

namespace GetBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class CommentsController : AdminBaseController
    {
        private readonly GetBlogDbContext _db;

        public CommentsController(GetBlogDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<ActionResult> Index(string filter = "pending")
        {
            IQueryable<Comment> commentsQuery = _db.Comments
                                                   .Include(c => c.Article)
                                                   .Include(c => c.Author);

            switch (filter.ToLower())
            {
                case "approved":
                    commentsQuery = commentsQuery.Where(c => c.IsApproved);
                    ViewBag.Filter = "approved";
                    break;
                case "all":
                    ViewBag.Filter = "all";
                    break;
                case "pending":
                default:
                    commentsQuery = commentsQuery.Where(c => !c.IsApproved);
                    ViewBag.Filter = "pending";
                    break;
            }

            var comments = await commentsQuery.OrderByDescending(c => c.CreatedDate).ToListAsync();
            return View(comments);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await _db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CommentId,Content,IsApproved")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var originalComment = await _db.Comments.FindAsync(comment.CommentId);
                if (originalComment == null)
                {
                    return HttpNotFound();
                }

                originalComment.Content = comment.Content;
                originalComment.IsApproved = comment.IsApproved;

                _db.Entry(originalComment).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Comment updated successfully.";
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ToggleApproval(int id)
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            comment.IsApproved = !comment.IsApproved;
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = comment.IsApproved ? "Comment has been approved." : "Comment has been un-approved.";

            return Redirect(Request.UrlReferrer?.ToString() ?? Url.Action("Index"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            var replies = await _db.Comments.Where(c => c.ParentCommentId == id).ToListAsync();
            if (replies.Any())
            {
                _db.Comments.RemoveRange(replies);
            }

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Comment and all its replies have been deleted.";

            return RedirectToAction("Index");
        }
    }
}