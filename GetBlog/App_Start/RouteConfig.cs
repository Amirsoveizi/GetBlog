using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GetBlog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "GetBlog.Controllers" }
            );

            routes.MapRoute(
    name: "ArticleDetailsExplicit",
    url: "article/{id}", 
    defaults: new { controller = "Articles", action = "Details" },
    constraints: new { id = @"\d+" } 
);
        }
    }
}
