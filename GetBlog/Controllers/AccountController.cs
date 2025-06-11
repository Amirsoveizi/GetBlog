using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;      // For DefaultAuthenticationTypes
using Microsoft.AspNet.Identity.Owin; // For GetUserManager
using GetBlog.Models;                 // Your ViewModels and ApplicationUser

namespace GetBlog.Controllers
{
    [Authorize] // Requires login for most actions, but Login/Register will be [AllowAnonymous]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: /Admin/Account/Login
        [AllowAnonymous] // Allow unauthenticated users to access the login page
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Shared/Login.cshtml",new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Shared/Login.cshtml", model);
            }

            var user = await UserManager.FindAsync(model.Username, model.Password);

            if (user != null)
            {
                var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.GetOwinContext().Authentication.SignIn(new Microsoft.Owin.Security.AuthenticationProperties { IsPersistent = model.RememberMe }, identity);

                if (await UserManager.IsInRoleAsync(user.Id, "Admin") || await UserManager.IsInRoleAsync(user.Id, "Editor"))
                {
                    return RedirectToLocal(returnUrl, isAdmin: true);
                }
                else
                {
                    return RedirectToLocal(returnUrl, isAdmin: false);
                }
            }
            else
            {
                ModelState.AddModelError("", "نام کاربری یا رمز ورود نامعتبر است.");
                return View("~/Views/Shared/Login.cshtml", model);
            }
        }

        // GET: /Admin/Account/Register
        [AllowAnonymous] // Allow unauthenticated users to access the register page
        public ActionResult Register()
        {
            return View("~/Views/Shared/Register.cshtml",new RegisterViewModel());
        }

        // POST: /Admin/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, FullName = model.FullName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                   
                    var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    HttpContext.GetOwinContext().Authentication.SignIn(new Microsoft.Owin.Security.AuthenticationProperties { IsPersistent = false }, identity);

                  
                    await UserManager.AddToRoleAsync(user.Id, "User");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View("~/Views/Shared/Register.cshtml",model);
        }


        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account", new { area = "Admin" }); // Redirect to login page
        }

        private ActionResult RedirectToLocal(string returnUrl, bool isAdmin = false)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (isAdmin)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
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