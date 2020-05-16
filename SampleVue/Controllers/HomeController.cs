using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dryv.Extensions;
using Dryv.Internal;
using Dryv.SampleVue.CustomValidation;
using Dryv.SampleVue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.SampleVue.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        [HttpPost]
        public Task<DryvResultMessage> Run(Address model)
        {
            return Task.FromResult(DryvResultMessage.Success);
        }
    }

    public class HomeController : Controller
    {
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