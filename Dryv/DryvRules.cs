using System.Collections.Generic;
using Dryv.Rules;

namespace Dryv
{
    public abstract class DryvRules
    {
        internal List<DryvCompiledRule> PropertyRules { get; } = new List<DryvCompiledRule>();

        public static DryvRules<TModel> For<TModel>() => new DryvRules<TModel>();
    }
}