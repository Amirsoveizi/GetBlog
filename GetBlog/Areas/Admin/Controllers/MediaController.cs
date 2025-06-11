
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GetBlog.Models;
using Microsoft.AspNet.Identity;

namespace GetBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Editor, Author")]
    public class MediaController : AdminBaseController
    {
        private readonly GetBlogDbContext _db;

        public MediaController(GetBlogDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<ActionResult> Index()
        {
            var mediaItems = await _db.MediaItems
                                      .OrderByDescending(m => m.UploadedDate)
                                      .ToListAsync();
            return View(mediaItems);
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase uploadFile, string altText)
        {
            if (uploadFile == null || uploadFile.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return View();
            }

            var uploadPath = Server.MapPath("~/images/");
            var fileExtension = Path.GetExtension(uploadFile.FileName).ToLower();
            var newFileName = Guid.NewGuid().ToString() + fileExtension;
            var fullPath = Path.Combine(uploadPath, newFileName);
            var publicUrl = "/images/" + newFileName;

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            try
            {
                await Task.Run(() => uploadFile.SaveAs(fullPath));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while saving the file.";
                return View();
            }

            var media = new Media
            {
                FileName = uploadFile.FileName,
                Url = publicUrl,
                MimeType = uploadFile.ContentType,
                AltText = altText,
                UploadedDate = DateTime.UtcNow,
                UploaderId = User.Identity.GetUserId()
            };

            _db.MediaItems.Add(media);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "File uploaded successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var media = await _db.MediaItems.FindAsync(id);
            if (media == null)
            {
                return HttpNotFound();
            }

            var filePath = Server.MapPath("~" + media.Url);

            _db.MediaItems.Remove(media);
            await _db.SaveChangesAsync();

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    await Task.Run(() => System.IO.File.Delete(filePath));
                }
            }
            catch (IOException ex)
            {
                TempData["ErrorMessage"] = "The database record was deleted, but the physical file could not be removed. Please check file permissions. Error: " + ex.Message;
            }

            TempData["SuccessMessage"] = "Media item deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}