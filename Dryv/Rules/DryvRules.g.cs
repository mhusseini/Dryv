using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dryv.Rules
{
    public partial class DryvRules<TModel>
    {

        public DryvRules<TModel> Disable<T1>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, bool>>,  Expression<Func<TModel,T1, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, bool>>,  Expression<Func<TModel,T1, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, bool>>,  Expression<Func<TModel,T1, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1,  bool>>,  Expression<Func<TModel,T1, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>, Expression<Func<TModel, T1, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>,  Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>, Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>,  Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, DryvValidationResult>, Func<TModel, T1, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, DryvValidationResult>, Func<TModel, T1, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, DryvValidationResult>, Func<TModel, T1, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, DryvValidationResult>, Func<TModel, T1, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, DryvValidationResult>, Func<TModel, T1, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, DryvValidationResult>, Func<TModel, T1, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>, Expression<Func<TModel, T1, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>,  Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>, Expression<Func<TModel, T1, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>,  Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, bool>>,  Expression<Func<TModel,T1, T2, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, bool>>,  Expression<Func<TModel,T1, T2, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, bool>>,  Expression<Func<TModel,T1, T2, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1, T2,  bool>>,  Expression<Func<TModel,T1, T2, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1, T2>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, DryvValidationResult>>, Expression<Func<TModel, T1, T2, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, DryvValidationResult>>,  Expression<Func<TModel, T1, T2,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1, T2>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, DryvValidationResult>>, Expression<Func<TModel, T1, T2,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, DryvValidationResult>>,  Expression<Func<TModel, T1, T2,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, DryvValidationResult>, Func<TModel, T1, T2, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, DryvValidationResult>, Func<TModel, T1, T2, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, DryvValidationResult>, Func<TModel, T1, T2, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, DryvValidationResult>, Func<TModel, T1, T2, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, DryvValidationResult>, Func<TModel, T1, T2, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, DryvValidationResult>, Func<TModel, T1, T2, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, DryvValidationResult>>, Expression<Func<TModel, T1, T2, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, DryvValidationResult>>,  Expression<Func<TModel, T1, T2,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, DryvValidationResult>>, Expression<Func<TModel, T1, T2, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, DryvValidationResult>>,  Expression<Func<TModel, T1, T2,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, bool>>,  Expression<Func<TModel,T1, T2, T3, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, bool>>,  Expression<Func<TModel,T1, T2, T3, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, bool>>,  Expression<Func<TModel,T1, T2, T3, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1, T2, T3,  bool>>,  Expression<Func<TModel,T1, T2, T3, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1, T2, T3>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1, T2, T3>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, DryvValidationResult>, Func<TModel, T1, T2, T3, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, DryvValidationResult>, Func<TModel, T1, T2, T3, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, DryvValidationResult>, Func<TModel, T1, T2, T3, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, DryvValidationResult>, Func<TModel, T1, T2, T3, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, DryvValidationResult>, Func<TModel, T1, T2, T3, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, DryvValidationResult>, Func<TModel, T1, T2, T3, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1, T2, T3, T4,  bool>>,  Expression<Func<TModel,T1, T2, T3, T4, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1, T2, T3, T4>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1, T2, T3, T4>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1, T2, T3, T4, T5,  bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1, T2, T3, T4, T5, T6,  bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7,  bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8,  bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9,  bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Disable(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
        public DryvRules<TModel> Disable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<bool>>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<bool>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,  bool>>,  Expression<Func<TModel,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<bool>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .DisableAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }
                
        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> Rule<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ServerRule<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Func<TModel, object>> property,
            Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>, Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                .Property(property)
                .ValidateAsync(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Func<TModel, object>> property,
            Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>> rule,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property)
                .Validate(rule);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

        public DryvRules<TModel> ClientRule<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Func<TModel, object>> property,
            Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>>, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
        {
            var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, DryvValidationResult>>,  Expression<Func<TModel, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                .Property(property);
            configure?.Invoke(builder);

            this.InternalRules.Add(builder.Rule);

            return this;
        }

	}
}
