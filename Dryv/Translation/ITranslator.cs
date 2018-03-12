using System.Linq.Expressions;

namespace Dryv.Translation
{
    public interface ITranslator
    {
        string Translate(Expression expression);

        void Translate(Expression expression, IndentingStringWriter writer);
    }
}