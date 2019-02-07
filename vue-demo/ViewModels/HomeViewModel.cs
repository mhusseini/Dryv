using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dryv;
using Microsoft.Extensions.Options;

namespace DryvDemo.ViewModels
{
    public class HomeViewModel
    {
        public static readonly DryvRules Rules = DryvRules.For<HomeViewModel>()
            .Rule(m => m.TaxId,
                m => !string.IsNullOrWhiteSpace(m.Company) && string.IsNullOrWhiteSpace(m.TaxId)
                    ? $"Die Steuernummer der Firma {m.Company} muss angegeben werden."
                    : DryvResultMessage.Success)
            .Rule<IOptions<DemoValidationOptions>>(m => m.PostalCode,
                (m, options) => !options.Value.IsAddressRequired && !m.IsAddressVisible || !string.IsNullOrWhiteSpace(m.PostalCode)
                    ? DryvResultMessage.Success
                    : "Die PLZ muss angegeben werden.")
            .ServerRule<IOptions<DemoValidationOptions>, ZipCodeValidator>(m => m.PostalCode,
                (m, options, validator) => !options.Value.IsAddressRequired && !m.IsAddressVisible && string.IsNullOrWhiteSpace(m.PostalCode)
                    ? Task.FromResult(DryvResultMessage.Success)
                    : validator.ValidateZipCode(m.PostalCode).ContinueWith(t => t.Result
                        ? DryvResultMessage.Success
                        : "Die PLZ ist nicht gültig."))
            .Rule<IOptions<DemoValidationOptions>, ZipCodeValidator>(m => m.City,
                (m, options, validator) => !options.Value.IsAddressRequired && !m.IsAddressVisible || !string.IsNullOrWhiteSpace(m.City)
                    ? DryvResultMessage.Success
                    : "Die Stadt muss angegeben werden.")
            .Rule(m => m.SelectionA, m => m.SelectionB,
                m => (m.SelectionA == null ? 0 : m.SelectionA.Count(i => i.IsSelected)) == (m.SelectionB == null ? 0 : m.SelectionB.Count(i => i.IsSelected))
                    ? DryvResultMessage.Success
                    : "Aus beiden listen müssen gleich viele Elemente ausgewählt werden.");

        [DryvRules]
        public string City { get; set; }

        public string Company { get; set; }

        public bool IsAddressVisible { get; set; }

        [Required]
        public string Name { get; set; }

        [DryvRules]
        public string PostalCode { get; set; }

        [DryvRules]
        public SelectionItem[] SelectionA { get; set; }

        [DryvRules]
        public SelectionItem[] SelectionB { get; set; }

        [DryvRules]
        public string TaxId { get; set; }

        public ModelChild Child { get; set; }
    }

    public class SelectionItem
    {
        public bool IsSelected { get; set; }
        public string Name { get; set; }
    }

    public class ModelChild
    {
        public static readonly DryvRules Rules = DryvRules.For<ModelChild>()
            .Rule(m => m.Name2,
                m => !"test".Equals(m.Name2)
                    ? "Der Name muss 'test' sein"
                    : DryvResultMessage.Success);

        [DryvRules]
        public string Name2 { get; set; }

        public ModelGrandChild Child { get; set; }
    }

    public class ModelGrandChild
    {
        public static readonly DryvRules Rules = DryvRules.For<ModelGrandChild>()
            .Rule(m => m.Name3,
                m => !"test".Equals(m.Name3)
                    ? "Der Name muss 'blah' sein"
                    : DryvResultMessage.Success);

        [DryvRules]
        public string Name3 { get; set; }
    }
}