using Dryv.Translation;
using System.Linq.Expressions;

namespace Dryv.MethodCallTranslation
{
    public class GenericTranslationContext : TranslationContext
    {
        public GenericTranslationContext()
        {
        }

        public GenericTranslationContext(TranslationContext context)
        {
            this.OptionsTypes = context.OptionsTypes;
            this.Writer = context.Writer;
            this.OptionDelegates = context.OptionDelegates;
        }

        public Expression Expression { get; set; }
        public bool Negated { get; set; }
        public Translator Translator { get; set; }
    }
}