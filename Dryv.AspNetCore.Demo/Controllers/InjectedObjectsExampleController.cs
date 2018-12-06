using DryvDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.Controllers
{
    public class InjectedObjectsExampleController : Controller
    {
        public IActionResult Index()
        {
            return this.View(new InjectedObjectsExampleVieWModel());
        }

        public IActionResult Post(InjectedObjectsExampleVieWModel model)
        {
            return this.View(nameof(this.Index), model);
        }
    }
}