namespace Dryv.Demo.Models
{
    public class Model4
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model4>()
            .Rule(m => m.Company,
                m => string.IsNullOrWhiteSpace(m.Company)
                    ? Options4.CompanyError
                    : m.Company.Length < Options4.CompanyNameLength
                        ? string.Format(Options4.CompanyLengthError, Options4.CompanyNameLength)
                        : DryvResult.Success);

        [DryvRules]
        public string Company { get; set; }
    }
}