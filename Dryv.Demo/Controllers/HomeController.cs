using Microsoft.AspNetCore.Mvc;

namespace Dryv.Demo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult GettingStarted()
        {
            return this.View();
        }
    }
}