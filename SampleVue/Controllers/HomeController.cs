using System.Linq;
using Dryv.Extensions;
using Dryv.SampleVue.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.SampleVue.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Post([FromBody]HomeModel _)
        {
            return this.ModelState.IsValid
                ? this.Json(new { success = true })
                : this.Json(new
                {
                    success = false,
                    errors = this.ModelState.ToDictionary(
                        s => string.Join('.', s.Key.Split('.').Select(v => v.ToCamelCase())),
                        s => s.Value.Errors.Select(e => e.ErrorMessage).First())
                });
        }
    }
}