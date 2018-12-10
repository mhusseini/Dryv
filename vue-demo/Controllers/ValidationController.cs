using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.Controllers
{
    public class ValidationController : Controller
    {
        public IActionResult Validate([FromQuery]string taxId)
        {
            return this.Json(taxId != "falsch");
        }
    }
}