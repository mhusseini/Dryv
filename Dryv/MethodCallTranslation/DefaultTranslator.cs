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

        public override bool Translate(MethodTranslationParameters parameters)
        {
            var objectType = parameters.Expression.Method.DeclaringType;

            if (this.translatorProvider
                .MethodCallTranslators
                .Where(t => t.SupportsType(objectType))
                .Any(t => t.Translate(parameters)))
            {
                return true;
            }

            throw new MethodCallNotAllowedException(parameters.Expression);
        }
    }
}