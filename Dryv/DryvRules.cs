using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv
{
    public class DryvRules
    {
        protected DryvRules()
        {
        }

        internal List<Expression> ModelRules { get; } = new List<Expression>();

        internal ConcurrentDictionary<string, List<Expression>> PropertyRules { get; } = new ConcurrentDictionary<string, List<Expression>>();

        public static Rules<TModel> For<TModel>() => new Rules<TModel>();

        public static Rules<TModel> For<TModel>(Expression<Func<TModel, DryvResult>> rule) => new Rules<TModel>(rule);
    }
}