using Dryv.Demo.Nav;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.Demo.Controllers
{
    public class ExtensionController : Controller
    {
        [Nav(Menu.ExtendingDryv)]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}