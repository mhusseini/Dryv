using System.Collections.Generic;

namespace Dryv.Demo.Models
{
    public class Model5
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model5>()
            .Rule(m => m.Name, m => string.IsNullOrWhiteSpace(m.Name) ? "na na na " : null);

        [DryvRules]
        public string Name { get; set; }

        public Model6 Child { get; set; }
        public IEnumerable<string> Texts { get; set; }
    }
}