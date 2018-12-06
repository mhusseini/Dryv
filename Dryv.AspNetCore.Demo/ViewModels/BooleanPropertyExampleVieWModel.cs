using System.ComponentModel.DataAnnotations;
using Dryv;

namespace DryvDemo.ViewModels
{
    public class BooleanPropertyExampleVieWModel
    {
        public static readonly DryvRules Rules = DryvRules.For<BooleanPropertyExampleVieWModel>()
            .Rule(m => m.Name,
                m => !m.IsOverlyManly || m.Name.EndsWith("or")
                    ? DryvResult.Success
                    : DryvResult.Error("Overly manly names must end with 'or'."))

            .Rule(m => m.HasNoFear,
                m => !m.IsOverlyManly || m.HasNoFear
                    ? DryvResult.Success
                    : DryvResult.Error("Must not have fear."));

        public bool IsOverlyManly { get; set; }

        [DryvRules]
        public bool HasNoFear { get; set; }

        [Required]
        [DryvRules]
        public string Name { get; set; }
    }
}