using System.ComponentModel.DataAnnotations;
using Dryv;

namespace DryvDemo.ViewModels
{
    public class BooleanPropertyExampleVieWModel
    {
        public static readonly DryvRules Rules = DryvRules.For<BooleanPropertyExampleVieWModel>()
            .Rule(m => m.Name,
                m => !m.IsOverlyManly || m.Name.EndsWith("or")
                    ? DryvResultMessage.Success
                    : DryvResultMessage.Error("Overly manly names must end with 'or'."))

            .Rule(m => m.HasNoFear,
                m => !m.IsOverlyManly || m.HasNoFear
                    ? DryvResultMessage.Success
                    : DryvResultMessage.Error("Must not have fear."));

        public bool IsOverlyManly { get; set; }

        [DryvRules]
        public bool HasNoFear { get; set; }

        [Required]
        [DryvRules]
        public string Name { get; set; }
    }
}