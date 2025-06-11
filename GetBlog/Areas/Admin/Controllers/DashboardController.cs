using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GetBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Editor, Author")]
    public class DashboardController : AdminBaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}