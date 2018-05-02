using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.Controllers
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