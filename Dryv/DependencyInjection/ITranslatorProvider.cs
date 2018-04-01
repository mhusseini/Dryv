using System.Collections.Generic;
using Dryv.Translation;

namespace Dryv.DependencyInjection
{
    internal interface ITranslatorProvider
    {
        IList<IMethodCallTranslator> MethodCallTranslators { get; }
        IList<ICustomTranslator> GenericTranslators { get; }
    }
}