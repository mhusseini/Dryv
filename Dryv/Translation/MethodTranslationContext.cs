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
            this.GroupName = context.GroupName;
            this.ModelType = context.ModelType;
            this.OptionDelegates = context.OptionDelegates;
            this.OptionsTypes = context.OptionsTypes;
            this.PropertyExpression = context.PropertyExpression;
            this.Writer = context.Writer;
        }

        public MethodCallExpression Expression { get; set; }
        public bool Negated { get; set; }
        public string Result { get; set; }
        public ITranslator Translator { get; set; }
    }
}