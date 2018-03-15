using Dryv.Demo.Models;
using Dryv.Demo.Nav;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.Demo.Controllers
{
    public class ExtensionController : Controller
    {
        [Nav(Menu.ExtendingDryv)]
        public IActionResult Index()
        {
            return this.View(new Customer());
        }


        [HttpPost]
        public IActionResult Index(Customer model)
        {
            return this.View(model);
        }
    }
}