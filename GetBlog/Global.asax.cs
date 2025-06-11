using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace GetBlog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var services = new ServiceCollection();
           
            var controllerTypes = Assembly.GetExecutingAssembly()
                                  .GetTypes()
                                  .Where(type => typeof(IController).IsAssignableFrom(type));

            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }


            services.AddScoped<Models.GetBlogDbContext>();

            var provider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

            ControllerBuilder.Current.SetControllerFactory(
            new DI.MsDiControllerFactory(provider));

        }
    }
}
