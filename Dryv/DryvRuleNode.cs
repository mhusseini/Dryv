using System;

namespace Dryv
{
    public class DryvRuleNode
    {
        public DryvRuleNode(string path, DryvRuleDefinition rule)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        public string Path { get; }

        public DryvRuleDefinition Rule { get; }
    }
}