using System.Collections.Generic;
using Dryv.MethodCallTranslation;

namespace Dryv.DependencyInjection
{
    internal interface ITranslatorProvider
    {
        IList<IMethodCallTranslator> MethodCallTranslators { get; }
        IList<IGenericTranslator> GenericTranslators { get; }
    }
}