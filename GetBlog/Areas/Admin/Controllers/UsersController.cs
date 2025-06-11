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
    public class UsersController : AdminBaseController
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private readonly GetBlogDbContext _db;

        public UsersController(GetBlogDbContext dbContext)
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
        public async Task<ActionResult> Index()
        {
            var users = await UserManager.Users
                                         .Include(u => u.Profile)
                                         .OrderBy(u => u.FullName)
                                         .ToListAsync();
            return View(users);
        }

        private void PopulateRolesViewBag(IEnumerable<string> selectedRoleIds = null)
        {
            var allRoles = RoleManager.Roles.OrderBy(r => r.Name).ToList();
            ViewBag.RolesList = new MultiSelectList(allRoles, "Id", "Name", selectedRoleIds);
        }

        public ActionResult Create()
        {
            PopulateRolesViewBag();
            return View(new ApplicationUser());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserName, Email, FullName")] ApplicationUser user, string Password, string[] roleIds, FormCollection form)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("Password", "A password is required.");
            }

            if (ModelState.IsValid)
            {
                var result = await UserManager.CreateAsync(user, Password);

                if (result.Succeeded)
                {
                    if (roleIds != null && roleIds.Any())
                    {
                        var selectedRoles = RoleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name);
                        await UserManager.AddToRolesAsync(user.Id, selectedRoles.ToArray());
                    }

                    var bio = form["Bio"];
                    if (!string.IsNullOrWhiteSpace(bio))
                    {
                        _db.AuthorProfiles.Add(new AuthorProfile { UserId = user.Id, Bio = bio });
                        await _db.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "User created successfully!";
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }

            PopulateRolesViewBag(roleIds);
            ViewBag.Bio = form["Bio"];
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
        public async Task<ActionResult> Edit([Bind(Include = "Id, UserName, Email, FullName")] ApplicationUser formUser, string[] roleIds, FormCollection form)
        {
            if (formUser.Id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userToUpdate = await UserManager.FindByIdAsync(formUser.Id);
            if (userToUpdate == null) return HttpNotFound();

            userToUpdate.FullName = formUser.FullName;
            userToUpdate.UserName = formUser.UserName;
            userToUpdate.Email = formUser.Email;

            var updateResult = await UserManager.UpdateAsync(userToUpdate);

            if (updateResult.Succeeded)
            {
                var userRoles = await UserManager.GetRolesAsync(userToUpdate.Id);
                var selectedRoles = roleIds != null ? RoleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToArray() : new string[] { };

                await UserManager.RemoveFromRolesAsync(userToUpdate.Id, userRoles.Except(selectedRoles).ToArray());
                await UserManager.AddToRolesAsync(userToUpdate.Id, selectedRoles.Except(userRoles).ToArray());

                var profile = await _db.AuthorProfiles.FindAsync(formUser.Id);
                var bio = form["Bio"];
                if (profile != null)
                {
                    profile.Bio = bio;
                    _db.Entry(profile).State = EntityState.Modified;
                }
                else if (!string.IsNullOrWhiteSpace(bio))
                {
                    _db.AuthorProfiles.Add(new AuthorProfile { UserId = formUser.Id, Bio = bio });
                }
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction("Index");

            }
            AddErrors(updateResult);

            PopulateRolesViewBag(roleIds);
            ViewBag.Profile = new AuthorProfile { Bio = form["Bio"] };
            return View(userToUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await UserManager.FindByIdAsync(id);
            if (user == null) return HttpNotFound();

            if (await _db.Articles.AnyAsync(a => a.AuthorId == id))
            {
                TempData["ErrorMessage"] = "Cannot delete this user as they have authored articles. Please reassign their content first.";
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
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
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