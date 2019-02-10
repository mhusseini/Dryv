using System.Collections.Generic;
using Dryv.Configuration;
using Dryv.Rules;

namespace Dryv
{
    public abstract class DryvRules
    {
        internal List<DryvRuleDefinition> PropertyRules { get; } = new List<DryvRuleDefinition>();

        public static DryvRules<TModel> For<TModel>() => new DryvRules<TModel>();
    }
}