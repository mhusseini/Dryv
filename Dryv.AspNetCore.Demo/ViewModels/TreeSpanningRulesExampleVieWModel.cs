using Dryv;

namespace DryvDemo.ViewModels
{
    public class TreeSpanningRulesExampleVieWModel
    {
        public static readonly DryvRules GrandChildRules = DryvRules.For<TreeSpanningRulesExampleVieWModelGrandChild>()
            .Rule(m => m.GrandChildName,
                m => string.IsNullOrWhiteSpace(m.GrandChildName)
                    ? $"{nameof(m.GrandChildName)} must be specified."
                    : DryvResult.Success);

        public static readonly DryvRules Rules = DryvRules.For<TreeSpanningRulesExampleVieWModel>()
            .Rule(m => m.Child.ChildName,
                m => string.IsNullOrWhiteSpace(m.Child.ChildName)
                    ? $"{nameof(m.Child.ChildName)} of must be specified."
                    : DryvResult.Success);

        public TreeSpanningRulesExampleVieWModelChild Child { get; set; }

        [DryvRules]
        public string Name { get; set; }
    }
}