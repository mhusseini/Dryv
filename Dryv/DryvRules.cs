using Dryv.Rules;

namespace Dryv
{
    public abstract class DryvRules
    {
        public static DryvRules<TModel> For<TModel>() => new DryvRules<TModel>();
    }
}