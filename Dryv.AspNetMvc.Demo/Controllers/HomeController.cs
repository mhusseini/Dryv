using System.Web.Mvc;
using DryvDemo.Areas.Examples.Models;
using DryvDemo.Models;

namespace DryvDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return this.View(new Model5
            {
                Name = "model5",
                Child = new Model6
                {
                    Name = "model6",
                    Child = new Model7
                    {
                        Name = "model7"
                    }
                }
            });
        }
    }
}