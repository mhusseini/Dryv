using System.ComponentModel.DataAnnotations;

namespace Dryv.Demo.Models
{
    public class Model1
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model1>()
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
}