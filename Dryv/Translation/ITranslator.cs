using System;
using System.Linq.Expressions;

namespace Dryv.Translation
{
    public interface ITranslator
    {
        Func<object[], string> Translate(Expression expression);

        void Translate(Expression expression, TranslationContext context);
    }
}