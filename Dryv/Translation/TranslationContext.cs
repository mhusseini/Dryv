using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv.Translation
{
    public class TranslationContext
    {
        public IndentingStringWriter Writer { get; set; }

        public IList<Type> OptionsTypes { get; set; }

        public IList<LambdaExpression> OptionDelegates { get; set; }
    }
}