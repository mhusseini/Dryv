using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;

namespace Dryv.SampleVue.Models
{
    public class Address
    {
        private static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
            .Rule(a => a.City, a => string.IsNullOrWhiteSpace(a.City) ? "Please enter a city." : null)
            .Rule(a => a.ZipCode, a => string.IsNullOrWhiteSpace(a.ZipCode) ? "Please enter a ZIP code." : null)
            .Rule<SampleOptions>(a => a.ZipCode, (a, o) => a.ZipCode.Trim().Length < o.ZipCodeLength ? $"ZIP code must have at least {o.ZipCodeLength} characters." : null)
            .Rule<AsyncValidator, SampleOptions>(a => a.ZipCode, (a, v, o) => v.ValidateZipCode(a.ZipCode, a.City, o.ZipCodeLength));

        [DryvRules]
        public string City { get; set; }

        [DryvRules]
        public string ZipCode { get; set; }
    }
}