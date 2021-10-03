using System.Collections.Generic;
using Dryv.Rules;

namespace Dryv
{
    public abstract class DryvRules
    {
        public IReadOnlyList<DryvCompiledRule> DisablingRules => this.InternalDisablingRules;
        public IReadOnlyList<DryvCompiledRule> Parameters => this.InternalParameters;
        public IReadOnlyList<DryvCompiledRule> ValidationRules => this.InternalValidationRules;
        internal List<DryvCompiledRule> InternalDisablingRules { get; } = new List<DryvCompiledRule>();
        internal List<DryvCompiledRule> InternalParameters { get; } = new List<DryvCompiledRule>();
        internal List<DryvCompiledRule> InternalValidationRules { get; } = new List<DryvCompiledRule>();

        public static DryvRules<TModel> For<TModel>() => new DryvRules<TModel>();
    }
}