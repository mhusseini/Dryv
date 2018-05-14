using System.Collections.Generic;

namespace Dryv.Demo.Models
{
    public class Model5
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model5>()
            .Rule(m => m.Name,
                m => m.Name != "haha" ? "must be haha" : null)
            .Rule(m => m.Child.Name,
                m => !string.IsNullOrWhiteSpace(m.Name) &&
                     !string.IsNullOrWhiteSpace(m.Child.Name) &&
                     m.Name != m.Child.Name
                    ? $"{nameof(Model6.Name)} of {nameof(Model6)} must be {m.Name}"
                    : null);

        [DryvRules]
        public string Name { get; set; }

        public Model6 Child { get; set; }

        public IEnumerable<Model8> Children { get; set; }
    }
}