using System.Linq.Expressions;
using Dryv.Translation;

namespace Dryv.MethodCallTranslation
{
    public class MethodTranslationParameters
    {
        public Translator Translator { get; set; }
        public MethodCallExpression Expression { get; set; }
        public IndentingStringWriter Writer { get; set; }
        public bool Negated { get; set; }
        public string Result { get; set; }
    }
}