using Dryv.Demo.Resources;

namespace Dryv.Demo.Models
{
    public class Model4
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model4>()
            .Rule(m => m.Company,
                m => string.IsNullOrWhiteSpace(m.Company)
                    ? LocalizedStrings.CompanyError
                    : DryvResult.Success);

        [DryvRules]
        public string Company { get; set; }
    }
}