using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dryv.Validation;

namespace Dryv.Rules
{
    partial class DryvRules<TModel>
    {

    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Func<TModel, DryvValidationResult> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            null,
            settings
            );
        return this;
    }

    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Func<TModel, Task<DryvValidationResult>> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            null,
            settings
            );
        return this;
    }
    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, DryvValidationResult> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            null,
            settings
            );
        return this;
    }

    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, Task<DryvValidationResult>> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            null,
            settings
            );
        return this;
    }
    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, DryvValidationResult> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            null,
            settings
            );
        return this;
    }

    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, Task<DryvValidationResult>> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            null,
            settings
            );
        return this;
    }
    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, DryvValidationResult> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            null,
            settings
            );
        return this;
    }

    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, Task<DryvValidationResult>> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            null,
            settings
            );
        return this;
    }
    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, DryvValidationResult> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            null,
            settings
            );
        return this;
    }

    public DryvRules<TModel> ServerRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, Task<DryvValidationResult>> rule, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            null,
            settings
            );
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, DryvValidationResult> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, Task<DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, DryvValidationResult> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, Task<DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, DryvValidationResult> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, Task<DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, DryvValidationResult> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, Task<DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, DryvValidationResult> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, Task<DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, TOptions2, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, TOptions2, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, TOptions2, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, TOptions2, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, TOptions2, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }
    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }

    public DryvRules<TModel> ServerRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
    {
        this.AddServer(rule,
            new[] { property1, property2, property3, property4, property5, },
            ruleSwitch,
            settings
            , typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
        return this;
    }

	}
}
