using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Dryv.AspNetMvc;
using DryvDemo.Areas.Examples;
using DryvDemo.Areas.Examples.Models;

namespace DryvDemo
{
    public static class AutofacMvcActivator
    {
        public static void Start()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            builder.RegisterDryv();

            builder.Register(_ => ExamplesConfiguration.CreateOptionsFromCookie<Options2>());
            builder.Register(_ => ExamplesConfiguration.CreateOptionsFromCookie<Options3>());

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}