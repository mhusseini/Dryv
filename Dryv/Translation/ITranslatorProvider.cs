using System.Collections.Generic;

namespace Dryv.Translation
{
    public interface ITranslatorProvider
    {
        ICollection<IMethodCallTranslator> MethodCallTranslators { get; }
        ICollection<ICustomTranslator> GenericTranslators { get; }
    }
}