using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dryv;
using Microsoft.Extensions.Options;

namespace DryvDemo.ViewModels
{
    public class HomeViewModel
    {
        public static readonly DryvRules Rules = DryvRules.For<HomeViewModel>()
            .Rule(m => m.TaxId,
                m => !string.IsNullOrWhiteSpace(m.Company) && string.IsNullOrWhiteSpace(m.TaxId)
                    ? $"Die Steuernummer der Firms {m.Company} muss angegeben werden."
                    : DryvResult.Success)
            .Rule<IOptions<DemoValidationOptions>, ZipCodeValidator>(m => m.PostalCode,
                (m, options, validator) => (!options.Value.IsAddressRequired && !m.IsAddressVisible) || !string.IsNullOrWhiteSpace(m.PostalCode)
                    ? DryvResult.Success
                    : "Die PLZ muss angegeben werden.")
            .Rule<IOptions<DemoValidationOptions>, ZipCodeValidator>(m => m.City,
                (m, options, validator) => (!options.Value.IsAddressRequired && !m.IsAddressVisible) || !string.IsNullOrWhiteSpace(m.City)
                    ? DryvResult.Success
                    : "Die Stadt muss angegeben werden.")
            .Rule(m => m.SelectionA,
                m => (m.SelectionA == null ? 0 : m.SelectionA.Count(i => i.IsSelected)) == (m.SelectionB == null ? 0 : m.SelectionB.Count(i => i.IsSelected))
                    ? DryvResult.Success
                    : "Aus beiden listen müssen gleich viele Elemente ausgewählt werden.");

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

        [DryvRules]
        public SelectionItem[] SelectionA { get; set; }

        [DryvRules]
        public SelectionItem[] SelectionB { get; set; }
    }

    public class SelectionItem
    {
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}