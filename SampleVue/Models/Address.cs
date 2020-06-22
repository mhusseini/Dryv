using System;
using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;
using Dryv.Validation;
using StringComparer = System.StringComparer;

namespace Dryv.SampleVue.Models
{
    public class Address
    {
        private static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
            .Rule(a => a.City, a => string.IsNullOrWhiteSpace(a.City) ? "Please enter a city." : null)
            .Rule(a => a.ZipCode, a => string.IsNullOrWhiteSpace(a.ZipCode) ? "Please enter a ZIP code." : null)
            .Rule<SampleOptions>(a => a.ZipCode, (a, o) => a.ZipCode.Trim().Length < o.ZipCodeLength ? $"ZIP code must have at least {o.ZipCodeLength} characters." : null)
            .Rule<AsyncValidator, SampleOptions>(a => a.ZipCode, (a, v, o) => v.ValidateZipCode(a.ZipCode, a.City, o.ZipCodeLength))
            .Rule(a => a.City, a => a.City.Contains("ass", StringComparison.OrdinalIgnoreCase) ? DryvResultMessage.Warning("Are you sure about this name?") : DryvResultMessage.Success);

        [DryvRules]
        public string City { get; set; }

        [DryvRules]
        public string ZipCode { get; set; }
    }
}