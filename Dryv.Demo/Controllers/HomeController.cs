using Dryv.Demo.Nav;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.Demo.Controllers
{
    public class HomeController : Controller
    {
        [Nav(Menu.Introduction)]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}