using Dryv.Rules;

namespace Dryv.SampleVue.Models
{
    public class Address
    {
        private static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
            .Rule(a => a.ZipCode, a => string.IsNullOrWhiteSpace(a.ZipCode) ? "Please enter a ZIP code." : null)
            .Rule(a => a.ZipCode, a => a.ZipCode.Trim().Length < 5 ? "ZIP code must have at least 5 characters." : null)
            .Rule(a => a.City, a => string.IsNullOrWhiteSpace(a.City) ? "Please enter a city." : null)
            .ServerRule<ZipCodeValidator>(a => a.ZipCode, a => a.City, (a, validator) => validator.ValidateZipCode(a.ZipCode, a.City));

        [DryvRules]
        public string ZipCode { get; set; }

        [DryvRules]
        public string City { get; set; }
    }
}