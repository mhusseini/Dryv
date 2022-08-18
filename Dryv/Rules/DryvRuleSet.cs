using System.Collections.Generic;

namespace Dryv.Rules
{
    public class DryvRuleSet
    {
        public string Name { get; set; }

        public List<DryvRule> Rules { get; set; } = new();
    }
}