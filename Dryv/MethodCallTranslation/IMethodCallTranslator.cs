using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dryv.MethodCallTranslation
{
    public interface IMethodCallTranslator
    {
        IList<Regex> TypeMatches { get; }
        bool Translate(MethodTranslationParameters options);
    }
}