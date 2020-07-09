using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace Dryv.Translation
{
    [DebuggerDisplay("{" + nameof(DebugView) + "}")]
    public class TranslationContext
    {
        internal static int ParameterCount = 0;

        private bool isAsync;
        private string DebugView => this.StringBuilder?.Length > 0 ? this.StringBuilder.ToString() : "PARENT: " + this.ParentContext?.DebugView;
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

        public IndentingStringWriter Writer { get; set; }
        internal StringBuilder StringBuilder { get; set; }
        public bool WhatIfMode { get; set; }
        public Func<Type, object> ServiceProvider { get; set; }

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
                Writer = sb == null ? this.Writer : new IndentingStringWriter(sb),
                StringBuilder = sb,
                WhatIfMode = this.WhatIfMode,
                ServiceProvider = this.ServiceProvider,
            };
        }
    }
}