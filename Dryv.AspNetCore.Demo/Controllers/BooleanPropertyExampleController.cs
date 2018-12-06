using DryvDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.Controllers
{
    public class BooleanPropertyExampleController : Controller
    {
        public IActionResult Index()
        {
            return this.View(new BooleanPropertyExampleVieWModel());
        }

        public IActionResult Post(BooleanPropertyExampleVieWModel model)
        {
            return this.View(nameof(this.Index), model);
        }
    }
}