using System.Linq.Expressions;

namespace Dryv.Translation
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
            this.ModelType = context.ModelType;
            this.PropertyExpression = context.PropertyExpression;
        }

        public MethodCallExpression Expression { get; set; }
        public bool Negated { get; set; }
        public string Result { get; set; }
        public ITranslator Translator { get; set; }
    }
}