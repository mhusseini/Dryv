using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dryv.Rules
{
    public partial class DryvRules<TModel> : DryvRules
    {
        public DryvRules<TModel> Disable(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, bool>>,  Expression<Func<TModel,Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, bool>>,  Expression<Func<TModel,Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, bool>>,  Expression<Func<TModel,Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, bool>>,  Expression<Func<TModel,Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        
        public DryvRules<TModel> Rule(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, DryvValidationResult>>,  Expression<Func<TModel,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, DryvValidationResult>>,  Expression<Func<TModel,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, DryvValidationResult>>,  Expression<Func<TModel,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, DryvValidationResult>>,  Expression<Func<TModel,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, DryvValidationResult>, Func<TModel, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, DryvValidationResult>, Func<TModel, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule(
            Expression<Func<TModel, object>> property,
            Func<TModel, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, DryvValidationResult>, Func<TModel, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, DryvValidationResult>, Func<TModel, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule(
            Expression<Func<TModel, object>> property,
            Func<TModel, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, DryvValidationResult>, Func<TModel, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, DryvValidationResult>, Func<TModel, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, DryvValidationResult>>,  Expression<Func<TModel,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, DryvValidationResult>>,  Expression<Func<TModel,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, DryvValidationResult>>,  Expression<Func<TModel,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, DryvValidationResult>>,  Expression<Func<TModel,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
    }
}