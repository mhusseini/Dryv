using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Dryv.AspNetMvc;
using Dryv.AspNetMvc.Demo;

namespace DryvDemo
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            UnityMvcActivator.Start();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DependencyResolver.Current.StartDryv();
        }
    }
}