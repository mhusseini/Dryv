using Dryv.AspNetCore.Extensions;
using Dryv.SampleVue.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.SampleVue.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Index([FromBody] HomeModel _)
        {
            return this.DryvValidationActionResult();

            //return this.ModelState.IsValid
            //    ? this.Json(new { success = true })
            //    : this.Json(new
            //    {
            //        success = false,
            //        results = this.ModelState.ToDictionary(
            //            s => string.Join('.', s.Key.Split
        }
    }
}