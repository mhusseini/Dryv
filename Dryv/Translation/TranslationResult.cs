using System;

namespace Dryv.Translation
{
    public class TranslationResult
    {
        public Func<object[], string> Factory { get; set; }
        public Type[] OptionTypes { get; set; }
        public string CodeTemplate { get; set; }
    }
}