using DryvDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.Controllers
{
    public class InjectedStaticsExampleController : Controller
    {
        public IActionResult Index()
        {
            return this.View(new InjectedStaticsExampleVieWModel());
        }

        public IActionResult Post(InjectedStaticsExampleVieWModel model)
        {
            return this.View(nameof(this.Index), model);
        }
    }
}