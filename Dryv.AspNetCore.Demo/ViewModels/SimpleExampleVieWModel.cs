using System.ComponentModel.DataAnnotations;
using Dryv;

namespace DryvDemo.ViewModels
{
    public class SimpleExampleVieWModel
    {
        public static readonly DryvRules Rules = DryvRules.For<SimpleExampleVieWModel>()
            .Rule(m => m.TaxId,
                m => !string.IsNullOrWhiteSpace(m.Company) && string.IsNullOrWhiteSpace(m.TaxId)
                    ? $"The tax ID for {m.Company} must be specified."
                    : DryvResultMessage.Success);

        [Required]
        public string Name { get; set; }

        public string Company { get; set; }

        [DryvRules]
        public string TaxId { get; set; }

    }
}