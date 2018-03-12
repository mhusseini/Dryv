using System.Linq.Expressions;
using Dryv.Translation;

namespace Dryv.MethodCallTranslation
{
    public class TranslationParameters
    {
        public Translator Translator { get; set; }
        public Expression Expression { get; set; }
        public IndentingStringWriter Writer { get; set; }
        public bool Negated { get; set; }
    }
}