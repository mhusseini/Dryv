using System.Threading.Tasks;
using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;

namespace Dryv.SampleVue.Models
{
    public class Address
    {
        private static DryvRules ValidationRules2 = DryvRules.For<HomeModel>()
            .DisableRules(m => m.BillingAddress, m => Task.FromResult(m.BillingEqualsShipping));

        private static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
        .Rule(a => a.ZipCode, a => string.IsNullOrWhiteSpace(a.ZipCode) ? "Please enter a ZIP code." : null)
        .Rule(a => a.ZipCode, a => a.ZipCode.Trim().Length < 5 ? "ZIP code must have at least 5 characters." : null)
        .Rule(a => a.City, a => string.IsNullOrWhiteSpace(a.City) ? "Please enter a city." : null)
        .Rule<AsyncValidator>(a => a.ZipCode, (a, v) => v.ValidateZipCode(a.ZipCode, a.City))
        ;

        [DryvRules]
        public string City { get; set; }

        [DryvRules]
        public string ZipCode { get; set; }
    }
}