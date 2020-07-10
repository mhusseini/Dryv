using System.Text;
using System.Threading.Tasks;
using Dryv.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    public abstract class DryvDynamicController : Controller
    {
        public async Task<IActionResult> JsonAsync(Task<object> task)
        {
            var value = await task;
            var options = this.HttpContext.RequestServices.GetService<IOptions<DryvOptions>>();
            var json = options.Value.JsonConversion(value);

            return this.Content(json, "application/json", Encoding.UTF8);
        }
        public IActionResult JsonSync(object value)
        {
            var options = this.HttpContext.RequestServices.GetService<IOptions<DryvOptions>>();
            var json = options.Value.JsonConversion(value);

            return this.Content(json, "application/json", Encoding.UTF8);
        }
    }
}