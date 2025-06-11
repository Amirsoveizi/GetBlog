using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GetBlog.DI
{
    public class MsDiControllerFactory : DefaultControllerFactory
    {
        private readonly ServiceProvider provider;

        public MsDiControllerFactory(ServiceProvider provider) => this.provider = provider;

        protected override IController GetControllerInstance(
            RequestContext requestContext, Type controllerType)
        {
            IServiceScope scope = this.provider.CreateScope();

            HttpContext.Current.Items[typeof(IServiceScope)] = scope;

            return (IController)scope.ServiceProvider.GetRequiredService(controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            base.ReleaseController(controller);

            var scope = HttpContext.Current.Items[typeof(IServiceScope)] as IServiceScope;

            scope?.Dispose();
        }
    }
}