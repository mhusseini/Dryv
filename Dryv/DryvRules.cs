using System.Collections.Generic;

namespace Dryv
{
    public abstract class DryvRules
    {
        internal List<DryvRuleDefinition> PropertyRules { get; } = new List<DryvRuleDefinition>();

        public static DryvRules<TModel> For<TModel>() => new DryvRules<TModel>();
    }
}