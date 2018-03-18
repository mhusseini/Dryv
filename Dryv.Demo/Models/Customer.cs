using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Dryv.Demo.Models
{
    public class TenantOptions
    {
        public bool IsEmptyCompanyAllowed { get; set; }
    }

    public class Customer
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Customer>()
            .Rule(m => m.TaxId,
                m => string.IsNullOrWhiteSpace(m.Company) || !string.IsNullOrWhiteSpace(m.TaxId)
                    ? DryvResult.Success
                    : $"The tax ID for {m.Company} must be specified.")
            .Rule(m => m.Company,
                m => m.Company.Equals("Oscorp", StringComparison.OrdinalIgnoreCase)
                    ? "Sorry, no evil corporations"
                    : DryvResult.Success)
            .Rule<IOptions<TenantOptions>>(
                m => m.Company,
                (m, o) => !o.Value.IsEmptyCompanyAllowed && string.IsNullOrWhiteSpace(m.Company)
                    ? "Company name is required"
                    : DryvResult.Success);

        [Required]
        public string Name { get; set; }

        [DryvRules]
        public string Company { get; set; }

        [DryvRules]
        public string TaxId { get; set; }

    }
}