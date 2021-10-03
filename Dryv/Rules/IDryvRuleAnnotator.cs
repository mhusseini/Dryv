using System.Linq.Expressions;

namespace Dryv.Rules
{
    public interface IDryvRuleAnnotator
    {
        void Annotate(DryvCompiledRule rule, LambdaExpression expression);
    }
}