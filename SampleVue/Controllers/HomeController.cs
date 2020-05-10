using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dryv.Extensions;
using Dryv.Internal;
using Dryv.SampleVue.CustomValidation;
using Dryv.SampleVue.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.SampleVue.Controllers
{
    //[Route("/")]
    public class HomeController : Controller
    {
        //private readonly DryvDynamicDelegatingControllerFactory codeGenerator;
        //private readonly DryvDynamicControllerRegistration controllerRegistration;

        //public HomeController(DryvDynamicControllerRegistration controllerRegistration, DryvDynamicDelegatingControllerFactory codeGenerator)
        //{
        //    this.controllerRegistration = controllerRegistration;
        //    this.codeGenerator = codeGenerator;
        //}

        //[HttpGet]
        public IActionResult Index()
        {
            //var v = new AsyncValidator();
            //Expression<Func<Address, Task<DryvResultMessage>>> e = a => v.ValidateZipCode(a.ZipCode, a.City);

            //var ass = this.codeGenerator.CreateControllerAssembly((MethodCallExpression)e.Body);

            //this.controllerRegistration.Register(ass);

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