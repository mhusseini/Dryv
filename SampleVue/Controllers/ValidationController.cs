using System.Threading.Tasks;
using Dryv.SampleVue.CustomValidation;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.SampleVue.Controllers
{
    [Route("[controller]")]
    public class ValidationController : Controller
    {
        private readonly AsyncValidator asyncValidator;

        public ValidationController(AsyncValidator asyncValidator) => this.asyncValidator = asyncValidator;

        [HttpGet]
        [Route(nameof(AsyncValidator.ValidateZipCode))]
        public Task<DryvResultMessage> ValidateZipCode(string zipCode, string city) => this.asyncValidator.ValidateZipCode(zipCode, city);
    }
}