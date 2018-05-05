using System.Web.Mvc;
using Dryv.AspNetMvc;
using DryvDemo.Areas.Examples;
using DryvDemo.Areas.Examples.Models;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;

namespace DryvDemo
{
    public static class SimpleInjectorMvcActivator
    {
        public static void Start()
        {
            var container = new Container
            {
                Options =
                {
                    DefaultScopedLifestyle = new WebRequestLifestyle()
                }
            };

            container.RegisterDryv();

            container.Register(ExamplesConfiguration.CreateOptionsFromCookie<Options2>, Lifestyle.Scoped);
            container.Register(ExamplesConfiguration.CreateOptionsFromCookie<Options3>, Lifestyle.Scoped);

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}