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
            return this.View(new HomeViewModel());
        }


        [HttpPost]
        public IActionResult Index(HomeViewModel model)
        {
            return this.View(model);
        }
    }
}