using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GetBlog.Models;

namespace GetBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : AdminBaseController
    {
        private readonly GetBlogDbContext _db;

        public SettingsController(GetBlogDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<ActionResult> Index()
        {
            var settingsList = await _db.Settings.ToListAsync();
            var settingsViewModel = settingsList.ToDictionary(s => s.SettingKey, s => s.SettingValue);
            return View(settingsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form)
        {
            var allSettings = await _db.Settings.ToListAsync();

            foreach (var key in form.AllKeys)
            {
                Setting setting = allSettings.FirstOrDefault(s => s.SettingKey == key);

                if (setting != null)
                {
                    setting.SettingValue = form[key];
                }
                else
                {
                    setting = new Setting
                    {
                        SettingKey = key,
                        SettingValue = form[key]
                    };
                    _db.Settings.Add(setting);
                }
            }

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Settings have been saved successfully!";
            return RedirectToAction("Index");
        }
    }
}