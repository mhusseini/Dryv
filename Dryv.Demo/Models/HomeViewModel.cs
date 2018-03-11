using System;
using System.ComponentModel.DataAnnotations;

namespace Dryv.Demo.Models
{
    public class HomeViewModel
    {
        public static readonly DryvRules Rules = DryvRules
            .For<HomeViewModel>()
            .Rule(m => m.TaxId,
                m => string.IsNullOrWhiteSpace(m.Company) || !string.IsNullOrWhiteSpace(m.TaxId)
                    ? DryvResult.Success
                    : $"The tax ID for {m.Company} must be specified.")
            .Rule(m => m.Company,
                m => m.Company.Equals("Oscorp", StringComparison.OrdinalIgnoreCase)
                    ? "Sorry, no evil corporations"
                    : DryvResult.Success);

        [Required]
        public string Name { get; set; }

        public string Company { get; set; }

        [DryvRules]
        public string TaxId { get; set; }

    }
}