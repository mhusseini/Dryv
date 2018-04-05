using System.Collections.Generic;

namespace Dryv.Demo.Models
{
    public class Model5
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model5>()
            .Rule(m => m.Child.Name,
                m => !string.IsNullOrWhiteSpace(m.Name) &&
                     !string.IsNullOrWhiteSpace(m.Child.Name) &&
                     m.Name != m.Child.Name
                    ? $"Name must be {m.Name}"
                    : null);

        [DryvRules]
        public string Name { get; set; }

        public Model6 Child { get; set; }
    }
}