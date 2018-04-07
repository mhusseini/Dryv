using System.ComponentModel;

namespace Dryv.Demo.Models
{
    public class Model6
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model6>()
            .Rule(m => m.Name, m => string.IsNullOrWhiteSpace(m.Name)
                ? "Name must not be empty"
                : null);
        public static readonly DryvRules Rules2 = DryvRules
            .For<Model6>()
            .Rule(m => m.Child2.Name, m => string.IsNullOrWhiteSpace(m.Child2.Name)
                ? "Name of childmust not be empty"
                : null);

        [DryvRules]
        [DisplayName("Name of Model6")]
        public string Name { get; set; }

        // public Model6 Child { get; set; }

        public Model7 Child2 { get; set; }
    }
}