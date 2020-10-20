using System;
using System.Threading.Tasks;
using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;
using Dryv.Validation;
using Microsoft.Extensions.Options;
using StringComparer = System.StringComparer;

namespace Dryv.SampleVue.Models
{
    public enum MyEnum
    {
        Value1,
        Value2
    }

    internal static class AddressValidation
    {
        public static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
            .Parameter<AsyncValidator, DryvValidationResult>("A", v => v.ValidateZipCode("A", "B"))
            //.Rule(a => a.City, a => string.IsNullOrWhiteSpace(a.City) ? "Please enter a city." : null)
            //.Rule(a => a.ZipCode, a => string.IsNullOrWhiteSpace(a.ZipCode) ? "Please enter a ZIP code." : null)
            //.Rule<IOptions<SampleOptions>>(a => a.ZipCode, (a, o) => a.ZipCode.Trim().Length < o.Value.ZipCodeLength ? $"ZIP code must have at least {o.Value.ZipCodeLength} characters." : null)
            //.Rule<AsyncValidator, IOptions<SampleOptions>>(a => a.ZipCode, (a, v, o) => v.ValidateZipCode(a.ZipCode, a.City, o.Value.ZipCodeLength))
            //.Rule(a => a.City, a => a.City.Contains("ass", StringComparison.OrdinalIgnoreCase) ? DryvValidationResult.Warning("Are you sure about this name?") : DryvValidationResult.Success)
            //.Rule<DryvParameters>(a => a.ZipCode, (a, p) => p.Get<string>("A") == a.ZipCode ? "error" : null)
            .Rule<AsyncValidator>(a => a.ZipCode, (a, v) => a.City == null ? null : v.IsValid(a.ZipCode) ? null : "error",
                async svc => await svc.ValidateZipCode("123", "xyz") == null)
            //.Rule(m=>m.Date, m=> m.Date < DateTime.Now ? "falsh" : null)
            ;
    }
    
    [DryvValidation(typeof(AddressValidation))]
    public class Address
    {
        public string City { get; set; }
        
        public DateTime Date { get; set; }

        public string ZipCode { get; set; }

        public MyEnum SomeEnum { get; set; }
    }
}