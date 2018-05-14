using Dryv;

namespace DryvDemo.Areas.Examples.Models
{
    public class Model3
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model3>()
            .Rule<IOptions<Options3>>(m => m.Company,
                (m, o) => string.IsNullOrWhiteSpace(m.Company)
                    ? "The company name must be specified."
                    : DryvResult.Success,
                o => o.Value.CompanyNameRequired);

        [DryvRules]
        public string Company { get; set; }
    }
}