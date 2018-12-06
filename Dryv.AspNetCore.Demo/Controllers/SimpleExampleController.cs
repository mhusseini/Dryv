using DryvDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.Controllers
{
    public class SimpleExampleController : Controller
    {
        public IActionResult Index()
        {
            return this.View(new SimpleExampleVieWModel());
        }

        public IActionResult Post(SimpleExampleVieWModel model)
        {
            return this.View(nameof(this.Index), model);
        }
    }
}