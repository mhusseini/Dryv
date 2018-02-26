using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    public class Rules<TModel> : DryvRules
    {
        internal Rules()
        {
        }

        internal Rules(Expression<Func<TModel, DryvResult>> rule)
        {
            this.ModelRules.Add(rule);
        }

        public Rules<TModel> Rule<TProperty>(
            Expression<Func<TModel, TProperty>> property1,
            Expression<Func<TModel, TProperty>> property2,
            Expression<Func<TModel, TProperty>> property3,
            Expression<Func<TModel, TProperty>> property4,
            Expression<Func<TModel, TProperty>> property5,
            Expression<Func<TModel, TProperty>> property6,
            Expression<Func<TModel, DryvResult>> rule)
        {
            this.Add(rule, property1, property2, property3, property4, property5, property6);
            return this;
        }

        public Rules<TModel> Rule<TProperty>(
            Expression<Func<TModel, TProperty>> property1,
            Expression<Func<TModel, TProperty>> property2,
            Expression<Func<TModel, TProperty>> property3,
            Expression<Func<TModel, TProperty>> property4,
            Expression<Func<TModel, TProperty>> property5,
            Expression<Func<TModel, DryvResult>> rule)
        {
            this.Add(rule, property1, property2, property3, property4, property5);
            return this;
        }

        public Rules<TModel> Rule<TProperty>(
            Expression<Func<TModel, TProperty>> property1,
            Expression<Func<TModel, TProperty>> property2,
            Expression<Func<TModel, TProperty>> property3,
            Expression<Func<TModel, TProperty>> property4,
            Expression<Func<TModel, DryvResult>> rule)
        {
            this.Add(rule, property1, property2, property3, property4);
            return this;
        }

        public Rules<TModel> Rule<TProperty>(
            Expression<Func<TModel, TProperty>> property1,
            Expression<Func<TModel, TProperty>> property2,
            Expression<Func<TModel, TProperty>> property3,
            Expression<Func<TModel, DryvResult>> rule)
        {
            this.Add(rule, property1, property2, property3);
            return this;
        }

        public Rules<TModel> Rule<TProperty>(
            Expression<Func<TModel, TProperty>> property1,
            Expression<Func<TModel, TProperty>> property2,
            Expression<Func<TModel, DryvResult>> rule)
        {
            this.Add(rule, property1, property2);
            return this;
        }

        public Rules<TModel> Rule<TProperty>(
            Expression<Func<TModel, TProperty>> property,
            Expression<Func<TModel, DryvResult>> rule)
        {
            this.Add(rule, property);
            return this;
        }

        public Rules<TModel> Rule(Expression<Func<TModel, DryvResult>> rule)
        {
            this.ModelRules.Add(rule);
            return this;
        }

        private void Add<TProperty>(
                                                                    Expression<Func<TModel, TProperty>> property,
            Expression<Func<TModel, DryvResult>> rule)
        {
            if (!(property.Body is MemberExpression memberExpression) ||
                !(memberExpression.Member is PropertyInfo propertyInfo))
            {
                return;
            }

            var expressions = this.PropertyRules.GetOrAdd(propertyInfo.Name, _ => new List<Expression>());
            expressions.Add(rule);
        }

        private void Add<TProperty>(
            Expression<Func<TModel, DryvResult>> rule,
            params Expression<Func<TModel, TProperty>>[] properties)
        {
            foreach (var property in properties)
            {
                this.Add(property, rule);
            }
        }
    }
}