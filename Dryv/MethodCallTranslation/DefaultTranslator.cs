using Dryv.DependencyInjection;
using System.Linq;

namespace Dryv.MethodCallTranslation
{
    internal class DefaultTranslator : MethodCallTranslator
    {
        private readonly ITranslatorProvider translatorProvider;

        public DefaultTranslator(ITranslatorProvider translatorProvider)
        {
            this.translatorProvider = translatorProvider;
        }

        public override bool Translate(MethodTranslationContext context)
        {
            var objectType = context.Expression.Method.DeclaringType;

            if (this.translatorProvider
                .MethodCallTranslators
                .Where(t => t.SupportsType(objectType))
                .Any(t => t.Translate(context)))
            {
                return true;
            }

            throw new MethodCallNotAllowedException(context.Expression);
        }
    }
}