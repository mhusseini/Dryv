$fn = "DryvRules.g.cs"

'using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dryv.Rules
{
    public partial class DryvRules<TModel>
    {
' | Out-File $fn


    $T = ""
    $sep = "";
    for($o = 1; $o -lt 11; $o++) {
        $T = $($T + $sep + "T" + $o)
        $sep = ", "
@"
                public DryvRules<TModel> Disable<T1>(
                    Expression<Func<TModel, object>> property,
                    Expression<Func<TModel, T1, DryvValidationResult>> rule,
                    Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>, Expression<Func<TModel, T1, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
                {
                    var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>,  Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                        .Property(property)
                        .Validate(rule);
                    configure?.Invoke(builder);
        
                    this.InternalDisablingRules.Add(builder.Rule);
        
                    return this;
                }
        
                public DryvRules<TModel> Disable<T1>(
                    Expression<Func<TModel, object>> property,
                    Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>, Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
                {
                    var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>,  Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                        .Property(property);
                    configure?.Invoke(builder);
        
                    this.InternalDisablingRules.Add(builder.Rule);
        
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
        
                    this.InternalValidationRules.Add(builder.Rule);
        
                    return this;
                }
        
                public DryvRules<TModel> Rule<T1>(
                    Expression<Func<TModel, object>> property,
                    Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>, Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
                {
                    var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>,  Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server)
                        .Property(property);
                    configure?.Invoke(builder);
        
                    this.InternalValidationRules.Add(builder.Rule);
        
                    return this;
                }
        
                public DryvRules<TModel> ServerRule<T1>(
                    Expression<Func<TModel, object>> property,
                    Action<DryvRuleBuilder<TModel, Func<TModel, T1, DryvValidationResult>, Func<TModel, T1, Task<DryvValidationResult>>, Func<TModel, bool>>> configure = null)
                {
                    var builder = new DryvRuleBuilder<TModel, Func<TModel, T1, DryvValidationResult>, Func<TModel, T1, Task<DryvValidationResult>>, Func<TModel, bool>>(DryvEvaluationLocation.Server)
                        .Property(property);
                    configure?.Invoke(builder);
        
                    this.InternalValidationRules.Add(builder.Rule);
        
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
        
                    this.InternalValidationRules.Add(builder.Rule);
        
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
        
                    this.InternalValidationRules.Add(builder.Rule);
        
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
        
                    this.InternalValidationRules.Add(builder.Rule);
        
                    return this;
                }
        
                public DryvRules<TModel> ClientRule<T1>(
                    Expression<Func<TModel, object>> property,
                    Action<DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>, Expression<Func<TModel, T1, Task<DryvValidationResult>>>, Func<TModel, bool>>> configure = null)
                {
                    var builder = new DryvRuleBuilder<TModel, Expression<Func<TModel, T1, DryvValidationResult>>,  Expression<Func<TModel, T1,Task<DryvValidationResult>>>, Func<TModel, bool>>(DryvEvaluationLocation.Client)
                        .Property(property);
                    configure?.Invoke(builder);
        
                    this.InternalValidationRules.Add(builder.Rule);
        
                    return this;
                }
"@.replace("T1", $T) | Add-Content $fn
}

'
	}
}' | Add-Content $fn
