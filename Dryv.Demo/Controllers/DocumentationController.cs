using Dryv.Demo.Nav;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.Demo.Controllers
{
    public class DocumentationController : Controller
    {
        [Nav(Menu.Documentation)]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}