using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GetBlog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;

namespace GetBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuthorsController : AdminBaseController
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private readonly GetBlogDbContext _db;

        public AuthorsController(GetBlogDbContext dbContext)
        {
            _db = dbContext;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }
        public ApplicationRoleManager RoleManager
        {
            get => _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            private set => _roleManager = value;
        }
        public ActionResult Index()
        {
            var authors = UserManager.Users
                                     .Include(u => u.Profile)
                                     .OrderBy(u => u.FullName)
                                     .ToList();
            return View(authors);
        }

        private void PopulateRolesViewBag(IEnumerable<string> selectedRoleIds = null)
        {
            var allRoles = RoleManager.Roles.OrderBy(r => r.Name).ToList();
            ViewBag.RolesList = new MultiSelectList(allRoles, "Id", "Name", selectedRoleIds);
        }

        public ActionResult Create()
        {
            PopulateRolesViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Username, Email, FullName")] ApplicationUser user, string Password, string[] roleIds, FormCollection form)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }

            if (ModelState.IsValid)
            {
                var result = await UserManager.CreateAsync(user, Password);

                if (result.Succeeded)
                {
                    if (roleIds != null)
                    {
                        var selectedRoles = RoleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name);
                        await UserManager.AddToRolesAsync(user.Id, selectedRoles.ToArray());
                    }

                    var authorProfile = new AuthorProfile
                    {
                        UserId = user.Id,
                        Bio = form["Bio"],
                        TwitterHandle = form["TwitterHandle"],
                        WebsiteUrl = form["WebsiteUrl"]
                    };
                    _db.AuthorProfiles.Add(authorProfile);
                    await _db.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Author created successfully!";
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }

            PopulateRolesViewBag(roleIds);
            ViewBag.Bio = form["Bio"];
            ViewBag.TwitterHandle = form["TwitterHandle"];
            ViewBag.WebsiteUrl = form["WebsiteUrl"];

            return View(user);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var profile = await _db.AuthorProfiles.FindAsync(id);
            ViewBag.Profile = profile ?? new AuthorProfile();

            var userRoleIds = user.Roles.Select(r => r.RoleId).ToArray();
            PopulateRolesViewBag(userRoleIds);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, string[] roleIds, FormCollection form)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userToUpdate = await UserManager.FindByIdAsync(id);
            if (userToUpdate == null) return HttpNotFound();

            userToUpdate.FullName = form["FullName"];
            userToUpdate.UserName = form["Username"];
            userToUpdate.Email = form["Email"];

            var updateResult = await UserManager.UpdateAsync(userToUpdate);

            if (updateResult.Succeeded)
            {
                var userRoles = await UserManager.GetRolesAsync(userToUpdate.Id);
                var selectedRoles = roleIds != null ? RoleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToArray() : new string[] { };

                await UserManager.RemoveFromRolesAsync(userToUpdate.Id, userRoles.Except(selectedRoles).ToArray());
                await UserManager.AddToRolesAsync(userToUpdate.Id, selectedRoles.Except(userRoles).ToArray());

                var profile = await _db.AuthorProfiles.FindAsync(id);
                if (profile != null)
                {
                    profile.Bio = form["Bio"];
                    profile.TwitterHandle = form["TwitterHandle"];
                    profile.WebsiteUrl = form["WebsiteUrl"];
                    _db.Entry(profile).State = EntityState.Modified;
                }
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Author updated successfully!";
                return RedirectToAction("Index");
            }
            AddErrors(updateResult);

            PopulateRolesViewBag(roleIds);
            ViewBag.Profile = new AuthorProfile { Bio = form["Bio"], TwitterHandle = form["TwitterHandle"], WebsiteUrl = form["WebsiteUrl"] };
            return View(userToUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await UserManager.FindByIdAsync(id);
            if (user == null) return HttpNotFound();

            if (_db.Articles.Any(a => a.AuthorId == id))
            {
                TempData["ErrorMessage"] = "Cannot delete this author because they have written articles. Please reassign their articles first.";
                return RedirectToAction("Index");
            }

            var profile = await _db.AuthorProfiles.FindAsync(id);
            if (profile != null)
            {
                _db.AuthorProfiles.Remove(profile);
                await _db.SaveChangesAsync();
            }

            var result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Author deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete author.";
                AddErrors(result);
            }

            return RedirectToAction("Index");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}