using Dryv.AspNetCore;
using Dryv.AspNetCore.Extensions;
using Dryv.SampleVue.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Dryv.SampleVue.Controllers
{
    [DryvValidationFilter]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Index([FromBody] HomeModel _)
        {
            var result = this.GetDryvValidationResults(DryvResultType.Error);
            if (result.Any())
            {
                return Json(new { success = false, messages = result });
            }

            return Ok();
        }
    }
}