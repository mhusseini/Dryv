using System.Collections.Generic;
using Dryv.Translation;

namespace Dryv.DependencyInjection
{
    public class TranslatorProvider : ITranslatorProvider
    {
        public IList<ICustomTranslator> GenericTranslators { get; } = new List<ICustomTranslator>();
        public IList<IMethodCallTranslator> MethodCallTranslators { get; } = new List<IMethodCallTranslator>();
    }
}