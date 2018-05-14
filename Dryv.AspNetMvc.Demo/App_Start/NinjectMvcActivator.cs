using System.Web.Mvc;
using Dryv.AspNetMvc;
using DryvDemo.Areas.Examples;
using DryvDemo.Areas.Examples.Models;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Mvc;

namespace DryvDemo
{
    public static class NinjectMvcActivator
    {
        public static void Start()
        {
            var kernel = new StandardKernel(new NinjectSettings());
            kernel.Unbind<ModelValidatorProvider>();

            kernel.RegisterDryv();

            kernel.Bind<IOptions<Options2>>().ToMethod(_ => ExamplesConfiguration.CreateOptionsFromCookie<Options2>()).InRequestScope();
            kernel.Bind<IOptions<Options3>>().ToMethod(_ => ExamplesConfiguration.CreateOptionsFromCookie<Options3>()).InRequestScope();

            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }
    }
}