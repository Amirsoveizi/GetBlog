using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GetBlog.Models; // Your models namespace
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin; // Required for GetOwinContext

namespace GetBlog.Areas.Admin.Controllers
{
    // Make sure only Admins can access this critical controller
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        // We use a private field for the RoleManager
        private ApplicationRoleManager _roleManager;

        // A public property to get the RoleManager from the Owin context
        public ApplicationRoleManager RoleManager
        {
            get
            {
                // If _roleManager is null, get it from the Owin context
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: Admin/Roles
        public ActionResult Index()
        {
            // Get a list of all roles
            var roles = RoleManager.Roles.OrderBy(r => r.Name).ToList();
            return View(roles);
        }

        // GET: Admin/Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);

                // Use the RoleManager to create the role
                var result = await RoleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Role created successfully!";
                    return RedirectToAction("Index");
                }

                // If it fails, add the errors to the model state
                AddErrors(result);
            }
            return View(roleViewModel);
        }

        // GET: Admin/Roles/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(new RoleViewModel(role));
        }

        // POST: Admin/Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleViewModel.Id);
                role.Name = roleViewModel.Name;

                // Use the RoleManager to update the role
                var result = await RoleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Role updated successfully!";
                    return RedirectToAction("Index");
                }

                AddErrors(result);
            }
            return View(roleViewModel);
        }

        // GET: Admin/Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Admin/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role.Users.Any())
            {
                TempData["ErrorMessage"] = "This role cannot be deleted because it is currently assigned to one or more users.";
                return RedirectToAction("Index");
            }

            var result = await RoleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Role deleted successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to delete role.";
            return RedirectToAction("Index");
        }

        // Helper method to add errors from IdentityResult to ModelState
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}