using DryvDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.Controllers
{
    public class TreeSpanningRulesExampleController : Controller
    {
        public IActionResult Index()
        {
            return this.View(new TreeSpanningRulesExampleVieWModel());
        }

        public IActionResult Post(TreeSpanningRulesExampleVieWModel model)
        {
            return this.View(nameof(this.Index), model);
        }
    }
}