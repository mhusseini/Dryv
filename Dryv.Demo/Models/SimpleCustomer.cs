using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Dryv.Demo.Models
{
    public class SimpleCustomer
    {
        public static readonly DryvRules Rules = DryvRules
            .For<SimpleCustomer>()
            .Rule(m => m.TaxId,
                m => string.IsNullOrWhiteSpace(m.Company) || !string.IsNullOrWhiteSpace(m.TaxId)
                    ? DryvResult.Success
                    : $"The tax ID for {m.Company} must be specified.");

        public string Company { get; set; }

        [Required]
        public string Name { get; set; }

        [DryvRules]
        public string TaxId { get; set; }
    }

    public class InlineOptions
    {
        public string CompanyPrefix { get; set; } = "Awesome";
    }

    public class ModelWithInlineOptions
    {
        public static readonly DryvRules Rules = DryvRules
            .For<ModelWithInlineOptions>()
            .Rule<IOptions<InlineOptions>>(m => m.Company,
                (m, o) => m.Company.StartsWith(o.Value.CompanyPrefix)
                    ? DryvResult.Success
                    : $"The company name must begin with '{o.Value.CompanyPrefix}'.");

        [DryvRules]
        public string Company { get; set; }
    }
    public class EnablingOptions
    {
        public bool CompanyNameRequired { get; set; } = false;
    }

    public class ModelWithEnablingOptions
    {
        public static readonly DryvRules Rules = DryvRules
            .For<ModelWithEnablingOptions>()
            .Rule<IOptions<EnablingOptions>>(m => m.Company,
                (m, o) => string.IsNullOrWhiteSpace(m.Company)
                    ? "The company name must be specified."
                    : DryvResult.Success,
                o => o.Value.CompanyNameRequired);

        [DryvRules]
        public string Company { get; set; }
    }
}