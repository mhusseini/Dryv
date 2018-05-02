using System.ComponentModel;

namespace Dryv.Demo.Models
{
    public class Model8
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model8>()
            .Rule(m => m.Name,
                m => !"wow".Equals(m.Name)
                    ? $"{nameof(Name)} of {nameof(Model8)} must be 'wow'"
                    : null);

        [DryvRules]
        [DisplayName("Name of Model8")]
        public string Name { get; set; }
    }
}