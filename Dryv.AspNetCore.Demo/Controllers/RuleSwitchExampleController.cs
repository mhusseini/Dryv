using DryvDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.Controllers
{
    public class RuleSwitchExampleController : Controller
    {
        public IActionResult Index()
        {
            return this.View(new RuleSwitchExampleVieWModel());
        }

        public IActionResult Post(RuleSwitchExampleVieWModel model)
        {
            return this.View(nameof(this.Index), model);
        }
    }
}