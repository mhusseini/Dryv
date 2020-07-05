using System.Threading.Tasks;
using Dryv.Rules;

namespace Dryv.SampleConsole.Models
{
    public class Address
    {
        private static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
            .Rule(a => a.ZipCode, a => !a.Deactivated && string.IsNullOrWhiteSpace(a.ZipCode) ? "Please enter a ZIP code." : null)
            .Rule(a => a.ZipCode, a => !a.Deactivated && a.ZipCode.Trim().Length < 5 ? "ZIP code must have at least 5 characters." : null)
            .Rule(a => a.City, a => !a.Deactivated && string.IsNullOrWhiteSpace(a.City) ? "Please enter a city." : null)
            .ServerRule<ZipCodeValidator>(a => a.ZipCode, a => a.City, (a, validator) => a.Deactivated
                ? Task.FromResult(DryvValidationResult.Success)
                : validator.ValidateZipCode(a.ZipCode, a.City));

        [DryvValidation]
        public string ZipCode { get; set; }

        [DryvValidation]
        public string City { get; set; }

        public bool Deactivated { get; set; }
    }
}