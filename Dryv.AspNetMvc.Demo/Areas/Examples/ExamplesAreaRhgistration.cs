using System.Web.Mvc;
using DryvDemo.Areas.Examples.Controllers;

namespace DryvDemo.Areas.Examples
{
    public class ExamplesAreaRhgistration : AreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context
                .MapRoute(
                    name: "Examples",
                    url: "Examples/{controller}/{action}",
                    defaults: new { action = nameof(ExamplesController.Index) }
                );
        }

        public override string AreaName => "Examples";
    }
}