using System.Linq.Expressions;

namespace Dryv.Translation
{
    public interface ITranslator
    {
        TranslationResult Translate(Expression expression);

        //void Translate(Expression expression, TranslationContext context);
    }
}