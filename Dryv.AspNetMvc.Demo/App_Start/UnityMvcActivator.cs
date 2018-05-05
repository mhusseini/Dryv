using System.Linq;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;
using Dryv.AspNetMvc;
using DryvDemo.Areas.Examples;
using DryvDemo.Areas.Examples.Models;

namespace DryvDemo
{
    public static class UnityMvcActivator
    {
        public static void Start()
        {
            var container = new UnityContainer();
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            container.RegisterDryv();

            container.RegisterType<Options2>(new InjectionFactory(_ => ExamplesConfiguration.CreateOptionsFromCookie<Options2>()));
            container.RegisterType<Options3>(new InjectionFactory(_ => ExamplesConfiguration.CreateOptionsFromCookie<Options3>()));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}