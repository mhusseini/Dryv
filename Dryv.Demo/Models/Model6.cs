using System.ComponentModel;

namespace Dryv.Demo.Models
{
    public class Model6
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model6>()
            .Rule(m => m.Name, m => string.IsNullOrWhiteSpace(m.Name)
                ? $"{nameof(Model6.Name)} of {nameof(Model6)} must not be empty"
                : null);

        public static readonly DryvRules Rules2 = DryvRules
            .For<Model7>()
            .Rule(m => m.Name, m => string.IsNullOrWhiteSpace(m.Name)
                ? $"{nameof(Model7.Name)} of {nameof(Model7)} not be empty"
                : null);

        [DryvRules]
        [DisplayName("Name of Model6")]
        public string Name { get; set; }

        public Model7 Child { get; set; }
    }
}