using System.Collections.Generic;

namespace Dryv
{
    public class TranslationResult
    {
        public Dictionary<string, List<TranslatedRule>> ValidationFunctions { get; internal set; }
        public Dictionary<string, List<TranslatedRule>> DisablingFunctions { get; internal set; }
        public Dictionary<string, object> Parameters { get; internal set; }
    }
}