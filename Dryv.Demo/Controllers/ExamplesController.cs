using Dryv.Demo.Models;
using Dryv.Demo.Nav;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.Demo.Controllers
{
    public class ExamplesController : Controller
    {
        [Nav(Menu.Examples)]
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Index(SimpleCustomer model)
        {
            return this.View(model);
        }
    }
}