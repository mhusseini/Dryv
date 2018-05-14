using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using DryvDemo.Areas.Examples.Models;

namespace DryvDemo.Areas.Examples.Controllers
{
    public class OptionsController : Controller
    {
        private readonly IOptions<Options2> options2;
        private readonly IOptions<Options3> options3;

        public OptionsController(
            IOptions<Options3> options3,
            IOptions<Options2> options2)
        {
            this.options3 = options3;
            this.options2 = options2;
        }

        [HttpPost]
        public ActionResult Example2(Options2 options)
        {
            this.options2.Value.CompanyPrefix = options.CompanyPrefix;
            return this.Process<Model2>(this.options2.Value);
        }

        [HttpPost]
        public ActionResult Example3(Options3 options)
        {
            this.options3.Value.CompanyNameRequired = options.CompanyNameRequired;
            return this.Process<Model3>(this.options3.Value);
        }

        private ActionResult Process<TModel>(object option, [CallerMemberName]string method = null)
        where TModel : class, new()
        {
            this.Response.AppendCookie(new HttpCookie(option.GetType().Name, Newtonsoft.Json.JsonConvert.SerializeObject(option)));
            return this.View($"~/Areas/Examples/Views/Examples/{method.ToLower()}.partial.cshtml", new TModel());
        }
    }
}