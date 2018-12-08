using Microsoft.AspNetCore.Mvc;
using DryvDemo.ViewModels;

namespace DryvDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View(new HomeViewModel());
        }

        [HttpPost]
        public IActionResult Post(HomeViewModel model)
        {
            return this.View(model);
        }
    }
}