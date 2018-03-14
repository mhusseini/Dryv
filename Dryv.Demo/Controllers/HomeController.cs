using Dryv.Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.Demo.Controllers
{
    public class HomeController : Controller
    {
        [Nav("Introduction")]
        public IActionResult Index()
        {
            return View(new HomeViewModel());
        }


        [HttpPost]
        public IActionResult Index(HomeViewModel model)
        {
            return View(model);
        }
    }
}