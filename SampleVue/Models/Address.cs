using System;
using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;
using Dryv.Validation;
using Microsoft.Extensions.Options;
using StringComparer = System.StringComparer;

namespace Dryv.SampleVue.Models
{
    public class Address
    {
        private static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
            .Rule<AsyncValidator>(a => a.ZipCode, (a, v) => "Z" + v.GetSomeString(a.ZipCode) + "C" + v.GetSomeString(a.City));
        //.Rule(a => a.City, a => string.IsNullOrWhiteSpace(a.City) ? "Please enter a city." : null)
        //.Rule(a => a.ZipCode, a => string.IsNullOrWhiteSpace(a.ZipCode) ? "Please enter a ZIP code." : null)
        //.Rule<IOptions<SampleOptions>>(a => a.ZipCode, (a, o) => a.ZipCode.Trim().Length < o.Value.ZipCodeLength ? $"ZIP code must have at least {o.Value.ZipCodeLength} characters." : null)
        //.Rule<AsyncValidator, IOptions<SampleOptions>>(a => a.ZipCode, (a, v, o) => v.ValidateZipCode(a.ZipCode, a.City, o.Value.ZipCodeLength))
        //.Rule(a => a.City, a => a.City.Contains("ass", StringComparison.OrdinalIgnoreCase) ? DryvValidationResult.Warning("Are you sure about this name?") : DryvValidationResult.Success);

        [DryvValidation]
        public string City { get; set; }

        [DryvValidation]
        public string ZipCode { get; set; }
    }
}