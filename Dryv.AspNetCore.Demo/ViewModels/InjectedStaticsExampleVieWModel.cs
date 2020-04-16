using Dryv;

namespace DryvDemo.ViewModels
{
    public class InjectedStaticsExampleVieWModel
    {
        public static readonly DryvRules Rules = DryvRules.For<InjectedStaticsExampleVieWModel>()
            .Rule(m => m.Company,
                m => string.IsNullOrWhiteSpace(m.Company)
                    ? InjectedStaticsExampleVieWModelStatics.CompanyError
                    : m.Company.Length < InjectedStaticsExampleVieWModelStatics.CompanyNameLength()
                        ? string.Format(InjectedStaticsExampleVieWModelStatics.CompanyLengthError, InjectedStaticsExampleVieWModelStatics.CompanyNameLength())
                        : DryvResultMessage.Success);

        [DryvRules]
        public string Company { get; set; }

    }

    public class InjectedStaticsExampleVieWModelStatics
    {
        public static string CompanyError { get; set; } = "The company name must be specified.";

        public const string CompanyLengthError = "The company name must be at least {0} characters long";

        public static int CompanyNameLength() => 3;
    }
}