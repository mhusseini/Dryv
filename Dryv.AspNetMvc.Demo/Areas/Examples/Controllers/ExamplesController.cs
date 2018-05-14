using System.Web.Mvc;
using DryvDemo.Areas.Examples.Models;

namespace DryvDemo.Areas.Examples.Controllers
{
    public class ExamplesController : Controller
    {
        [HttpPost]
        public ActionResult Example1(Model1 model) => this.View("example1.partial", model);

        [HttpPost]
        public ActionResult Example2(Model2 model) => this.View("example2.partial", model);

        [HttpPost]
        public ActionResult Example3(Model3 model) => this.View("example3.partial", model);

        [HttpPost]
        public ActionResult Example4(Model4 model) => this.View("example4.partial", model);

        [HttpPost]
        public ActionResult Example5(Model5 model) => this.View("example5.partial", model);

        //[HttpPost]
        //public ActionResult Example6(Model8 model) => this.View("example6.partial", model);

        [HttpGet]
        public ActionResult Index() => this.View();
    }
}