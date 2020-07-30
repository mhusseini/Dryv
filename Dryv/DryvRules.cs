using System.Collections.Concurrent;
using System.Collections.Generic;
using Dryv.Rules;

namespace Dryv
{
    public abstract class DryvRules
    {
        internal List<DryvCompiledRule> Parameters { get; } = new List<DryvCompiledRule>();
        internal List<DryvCompiledRule> ValidationRules { get; } = new List<DryvCompiledRule>();
        internal List<DryvCompiledRule> DisablingRules { get; } = new List<DryvCompiledRule>();

        public static DryvRules<TModel> For<TModel>() => new DryvRules<TModel>();
    }
}