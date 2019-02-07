using System.Threading.Tasks;
using Dryv;
using Microsoft.AspNetCore.Mvc;
using DryvDemo.ViewModels;

namespace DryvDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View(new HomeViewModel
            {
                SelectionA = new[]
                {
                    new SelectionItem{Name = "Selection A1"},
                    new SelectionItem{Name = "Selection A2"},
                },
                SelectionB = new[]
                {
                    new SelectionItem{Name = "Selection B1"},
                    new SelectionItem{Name = "Selection B2"},
                }
            });
        }
        public IActionResult Index2()
        {
            return this.View(new HomeViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeViewModel model)
        {
            return this.View(model);
        }
    }
}