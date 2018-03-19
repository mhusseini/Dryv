using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    public class DryvRules
    {
        protected DryvRules()
        {
        }

        internal List<Expression> ModelRules { get; } = new List<Expression>();

        internal ConcurrentDictionary<PropertyInfo, List<DryvRule>> PropertyRules { get; } = new ConcurrentDictionary<PropertyInfo, List<DryvRule>>();

        public static Rules<TModel> For<TModel>() => new Rules<TModel>();

        public static Rules<TModel> For<TModel>(Expression<Func<TModel, DryvResult>> rule) => new Rules<TModel>(rule);
    }
}