using Dryv;

namespace DryvDemo.ViewModels
{
    public class TreeSpanningRulesExampleVieWModelChild
    {
        [DryvRules]
        public string ChildName { get; set; }

        public TreeSpanningRulesExampleVieWModelGrandChild Child { get; set; }
    }
}