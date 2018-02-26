using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv
{
    public class Rules
    {
        protected Rules()
        {
        }

        internal List<Expression> ModelRules { get; } = new List<Expression>();
        internal ConcurrentDictionary<string, List<Expression>> PropertyRules { get; } = new ConcurrentDictionary<string, List<Expression>>();

        public static Rules<TModel> For<TModel>()
        {
            return new Rules<TModel>();
        }

        public static Rules<TModel> For<TModel>(Expression<Func<TModel, Result>> rule)
        {
            return new Rules<TModel>(rule);
        }
    }
}