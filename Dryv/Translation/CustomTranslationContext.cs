using System;
using System.Collections.Generic;
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
            this.ModelType = context.ModelType;
            this.PropertyExpression = context.PropertyExpression;
        }

        public Expression Expression { get; set; }
        public bool Negated { get; set; }
        public ITranslator Translator { get; set; }
    }
}