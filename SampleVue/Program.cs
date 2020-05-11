using System;
using System.Linq;
using System.Reflection;
using Dryv.SampleVue.CustomValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;

namespace Dryv.SampleVue
{
    public class TestFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var dtoType = context.Controller.GetType().Assembly.DefinedTypes.First(t => t.Name.Contains("Dto"));
            dynamic controller = context.Controller;
            dynamic dto = Activator.CreateInstance(dtoType);

            dto.zipCode = "124";
            dto.city = "xyv";

            var result = controller.ValidateZipCode(dto).Result;

            base.OnActionExecuting(context);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
