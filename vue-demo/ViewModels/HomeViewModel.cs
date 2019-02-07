using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dryv;
using Microsoft.Extensions.Options;

namespace DryvDemo.ViewModels
{
    public class HomeViewModel
    {
        public static readonly DryvRules Rules = DryvRules.For<HomeViewModel>()
            .Rule(m => m.TaxId,
                m => !string.IsNullOrWhiteSpace(m.Company) && string.IsNullOrWhiteSpace(m.TaxId)
                    ? $"The tax ID for {m.Company} must be specified."
                    : DryvResult.Success)
            .Rule<IOptions<DemoValidationOptions>, ZipCodeValidator>(m => m.PostalCode,
                (m, options, validator) => (!options.Value.IsAddressRequired && !m.IsAddressVisible) || !string.IsNullOrWhiteSpace(m.PostalCode)
                    ? DryvResult.Success
                    : "The ZIP code must be specified.")
            .Rule<IOptions<DemoValidationOptions>, ZipCodeValidator>(m => m.City,
                (m, options, validator) => (!options.Value.IsAddressRequired && !m.IsAddressVisible) || !string.IsNullOrWhiteSpace(m.City)
                    ? DryvResult.Success
                    : "The city must be specified.");

        [Required]
        public string Name { get; set; }

        public string Company { get; set; }

        [DryvRules]
        public string TaxId { get; set; }

        public bool IsAddressVisible { get; set; }

        [DryvRules]
        public string PostalCode { get; set; }

        [DryvRules]
        public string City { get; set; }
    }
}