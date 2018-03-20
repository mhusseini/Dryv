using Dryv.Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dryv.Demo.Controllers
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
        public IActionResult Example2(Options2 options)
        {
            this.options2.Value.CompanyPrefix = options.CompanyPrefix;
            return this.View("~/Views/Examples/example2.partial.cshtml", new Model2());
        }

        [HttpPost]
        public IActionResult Example3(Options3 options)
        {
            this.options3.Value.CompanyNameRequired = options.CompanyNameRequired;
            return this.View("~/Views/Examples/example3.partial.cshtml", new Model3());
        }

        [HttpPost]
        public IActionResult Example4()
        {
            return this.View("~/Views/Examples/example4.partial.cshtml", new Model4());
        }
    }
}