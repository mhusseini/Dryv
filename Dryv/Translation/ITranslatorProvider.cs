using System.Collections.Generic;

namespace Dryv.Translation
{
    public interface ITranslatorProvider
    {
        IList<IMethodCallTranslator> MethodCallTranslators { get; }
        IList<ICustomTranslator> GenericTranslators { get; }
    }
}