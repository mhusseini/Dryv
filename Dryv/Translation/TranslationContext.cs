using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using Dryv.Rules;

namespace Dryv.Translation
{
    [DebuggerDisplay("{" + nameof(DebugView) + "}")]
    public class TranslationContext
    {
        internal static int ParameterCount = 0;
        private bool isAsync;
        public Dictionary<object, object> CustomData { get; private set; } = new Dictionary<object, object>();
        private string DebugView => this.StringBuilder?.Length > 0 ? this.StringBuilder.ToString() : "PARENT: " + this.ParentContext?.DebugView;
        public List<Func<Expression, TranslationContext, bool>> DynamicTranslation { get; private set; } = new List<Func<Expression, TranslationContext, bool>>();
        public string GroupName { get; set; }

        public bool IsAsync
        {
            get => this.isAsync;
            set
            {
                this.isAsync = value;

                if (this.ParentContext != null)
                {
                    this.ParentContext.IsAsync = value;
                }
            }
        }

        public Type ModelType { get; set; }
        public IDictionary<int, OptionDelegate> OptionDelegates { get; set; }
        public IList<Type> OptionsTypes { get; set; }
        public TranslationContext ParentContext { get; set; }
        public Expression PropertyExpression { get; set; }
        public DryvCompiledRule Rule { get; set; }
        public ITranslator Translator { get; set; }
        public bool WhatIfMode { get; set; }
        public StringWriter Writer { get; set; }
        internal StringBuilder StringBuilder { get; set; }

        public virtual T Clone<T>(StringBuilder sb = null)
        where T : TranslationContext, new()
        {
            return new T
            {
                ParentContext = this,
                GroupName = this.GroupName,
                ModelType = this.ModelType,
                OptionDelegates = this.OptionDelegates,
                OptionsTypes = this.OptionsTypes,
                PropertyExpression = this.PropertyExpression,
                Writer = sb == null ? this.Writer : new StringWriter(sb),
                StringBuilder = sb,
                WhatIfMode = this.WhatIfMode,
                Rule = this.Rule,
                Translator = this.Translator,
                CustomData = this.CustomData,
                DynamicTranslation = this.DynamicTranslation,
            };
        }
    }
}