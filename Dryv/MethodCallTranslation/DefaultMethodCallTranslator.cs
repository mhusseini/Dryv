using System.Linq;
using System.Reflection;
using Dryv.DependencyInjection;

namespace Dryv.MethodCallTranslation
{
    internal class DefaultMethodCallTranslator : MethodCallTranslator
    {
        private static readonly MemberInfo ErrorMember = typeof(DryvResult).GetMember("Error").First();
        private readonly ITranslatorProvider translatorProvider;

        public DefaultMethodCallTranslator(ITranslatorProvider translatorProvider)
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

            switch (parameters.Expression.Method.Name)
            {
                case nameof(DryvResult.Error):
                    if (parameters.Expression.Method != ErrorMember)
                    {
                        throw new MethodCallNotAllowedException(parameters.Expression);
                    }

                    parameters.Writer.Write(parameters.Expression.Arguments.First());
                    return true;

                default:
                    throw new MethodCallNotAllowedException(parameters.Expression);
            }
        }
    }
}