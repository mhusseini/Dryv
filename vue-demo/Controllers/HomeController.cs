using System.Threading.Tasks;
using DryvDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ZipCodeValidator zipCodeValidator;

        public HomeController(ZipCodeValidator zipCodeValidator)
        {
            this.zipCodeValidator = zipCodeValidator;
        }

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

        [HttpPost]
        public async Task<IActionResult> Index(HomeViewModel model)
        {
            return this.View(model);
        }

        public IActionResult Index2()
        {
            return this.View(new HomeViewModel());
        }

        public async Task<IActionResult> ValidateZip(string zip)
        {
            return this.Json(await this.zipCodeValidator.ValidateZipCode(zip));
        }
    }
}