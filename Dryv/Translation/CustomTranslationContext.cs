using System.Linq.Expressions;

namespace Dryv.Translation
{
    public class CustomTranslationContext : TranslationContext
    {
        public CustomTranslationContext()
        {
        }

        public CustomTranslationContext(TranslationContext context)
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