using System;
using Dryv.Rules;

namespace Dryv.RuleDetection
{
    public class DryvRuleTreeNode
    {
        public DryvRuleTreeNode(string path, DryvCompiledRule rule)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        public string Path { get; }

        public DryvCompiledRule Rule { get; }
    }
}