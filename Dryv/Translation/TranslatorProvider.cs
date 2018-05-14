using System.Collections.Generic;

namespace Dryv.Translation
{
    public class TranslatorProvider : ITranslatorProvider
    {
        public IList<ICustomTranslator> GenericTranslators { get; } = new List<ICustomTranslator>();
        public IList<IMethodCallTranslator> MethodCallTranslators { get; } = new List<IMethodCallTranslator>();
    }
}