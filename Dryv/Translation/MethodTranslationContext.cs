using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv.Translation
{
    public class MethodTranslationContext : TranslationContext
    {
        public MethodCallExpression Expression { get; set; }
        public bool Negated { get; set; }
        public string Result { get; set; }
    }
}