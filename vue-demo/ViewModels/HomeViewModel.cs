using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dryv;
using Dryv.Rules;
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
            .ClientRule<IOptions<DemoValidationOptions>>(m => m.PostalCode,
                (m, o) => DryvClientCode.Raw("ajax(") + $"Home/ValidateZip/{m.PostalCode}" + DryvClientCode.Raw(", 'Die PLZ ist nicht gültig.')"))

            .Rule<IOptions<DemoValidationOptions>, ZipCodeValidator>(m => m.City,
                (m, options, validator) => !options.Value.IsAddressRequired && !m.IsAddressVisible || !string.IsNullOrWhiteSpace(m.City)
                    ? DryvResultMessage.Success
                    : "Die Stadt muss angegeben werden.")
            .Rule(m => m.SelectionGroup,
                m => (m.SelectionA == null ? 0 : m.SelectionA.Count(i => i.IsSelected)) == (m.SelectionB == null ? 0 : m.SelectionB.Count(i => i.IsSelected))
                    ? DryvResultMessage.Success
                    : "Aus beiden listen müssen gleich viele Elemente ausgewählt werden.")
            .Rule(m => m.Child.Name2,
                m => string.IsNullOrWhiteSpace(m.Company) || string.Equals(m.Child.Name2, m.Company)
                    ? DryvResultMessage.Success
                    : $"Der Name muss '{m.Company}' sein.");

        [DryvRules]
        public string City { get; set; }

        public string Company { get; set; }

        public bool IsAddressVisible { get; set; }

        [Required]
        public string Name { get; set; }

        [DryvRules]
        public string PostalCode { get; set; }

        [DryvRules]
        public string SelectionGroup { get; set; }

        public SelectionItem[] SelectionA { get; set; }

        public SelectionItem[] SelectionB { get; set; }

        [DryvRules]
        public string TaxId { get; set; }

        public ModelChild Child { get; set; }
    }

    public class SelectionItem
    {
        public static readonly DryvRules Rules = DryvRules.For<SelectionItem>()
            .Rule(m => m.Name,
                m => "doof".Equals(m.Name, StringComparison.OrdinalIgnoreCase)
                    ? "Der Name darf nicht 'doof' sein."
                    : DryvResultMessage.Success);

        public bool IsSelected { get; set; }

        [DryvRules]
        public string Name { get; set; }
    }

    public class ModelChild
    {
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