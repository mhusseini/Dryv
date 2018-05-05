#define ninject

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Dryv.AspNetMvc;

namespace DryvDemo
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
#if unity
            UnityMvcActivator.Start();
#elif autofac
            AutofacMvcActivator.Start();
#elif ninject
            NinjectMvcActivator.Start();
#elif simpleinjector
            SimpleInjectorMvcActivator.Start();
#endif

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DependencyResolver.Current.StartDryv();
        }
    }
}