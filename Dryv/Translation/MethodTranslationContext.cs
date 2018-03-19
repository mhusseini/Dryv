using Dryv.Translation;
using System.Linq.Expressions;

namespace Dryv.MethodCallTranslation
{
    public class MethodTranslationContext : TranslationContext
    {
        public MethodTranslationContext()
        {
        }

        public MethodTranslationContext(TranslationContext context)
        {
            this.OptionsTypes = context.OptionsTypes;
            this.Writer = context.Writer;
            this.OptionDelegates = context.OptionDelegates;
        }

        public MethodCallExpression Expression { get; set; }
        public bool Negated { get; set; }
        public string Result { get; set; }
        public Translator Translator { get; set; }
    }
}