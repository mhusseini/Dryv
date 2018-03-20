using Microsoft.Extensions.Options;

namespace Dryv.Demo.Models
{
    public class Model2
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model2>()
            .Rule<IOptions<Options2>>(m => m.Company,
                (m, o) => m.Company.StartsWith(o.Value.CompanyPrefix)
                    ? DryvResult.Success
                    : $"The company name must begin with '{o.Value.CompanyPrefix}'.");

        [DryvRules]
        public string Company { get; set; }
    }
}