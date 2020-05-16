﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv.Translation
{
    public class TranslationContext
    {
        public Type ModelType { get; set; }
        public IDictionary<int, OptionDelegate> OptionDelegates { get; set; }
        public IList<Type> OptionsTypes { get; set; }
        public Expression PropertyExpression { get; set; }

        public IndentingStringWriter Writer { get; set; }
        public List<Type> ClientCodeModifiers { get; set; } = new List<Type>();
    }
}