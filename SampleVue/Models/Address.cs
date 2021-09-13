using System;
using System.Threading.Tasks;
using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;
using Dryv.Validation;
using Microsoft.Extensions.Options;
using StringComparer = System.StringComparer;

namespace Dryv.SampleVue.Models
{
    internal static class AddressValidation
    {
        public static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
            .Rule<AsyncValidator>(a => a.ZipCode, (a, v) => v.ValidateZipCode(a.ZipCode, a.City, 5));
    }
    
    [DryvValidation(typeof(AddressValidation))]
    public class Address
    {
        public string City { get; set; }

        public string ZipCode { get; set; }
    }
}