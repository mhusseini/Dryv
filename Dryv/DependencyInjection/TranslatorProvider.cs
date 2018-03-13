using System.Collections.Generic;
using Dryv.MethodCallTranslation;

namespace Dryv.DependencyInjection
{
    public class TranslatorProvider : ITranslatorProvider
    {
        public IList<IGenericTranslator> GenericTranslators { get; } = new List<IGenericTranslator>();
        public IList<IMethodCallTranslator> MethodCallTranslators { get; } = new List<IMethodCallTranslator>();
    }
}