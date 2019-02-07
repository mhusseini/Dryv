using Dryv;
using Microsoft.Extensions.Options;

namespace DryvDemo.ViewModels
{
    //
    // If a rule switch returns false, the rule will not be applied.
    // Server-side validation will always succeed and client-side
    // validation will not get generated.
    //
    public class RuleSwitchExampleVieWModel
    {
        public static readonly DryvRules Rules = DryvRules.For<RuleSwitchExampleVieWModel>()
            .Rule<IOptions<RuleSwitchExampleVieWModelOptions>>(m => m.Company,
                (m, o) => string.IsNullOrWhiteSpace(m.Company)
                    ? "The company name must be specified."
                    : DryvResultMessage.Success,
                o => o.Value.CompanyNameRequired);

        [DryvRules]
        public string Company { get; set; }

    }
}