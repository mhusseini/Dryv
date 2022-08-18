using System.Collections.Generic;
using Dryv.Rules;

namespace Dryv
{
    public abstract class DryvRules
    {
        internal IReadOnlyList<DryvRule> Rules => this.InternalRules;

        internal List<DryvRule> InternalRules { get; } = new();
        public static DryvRules<TModel> For<TModel>() => new();
    }
}