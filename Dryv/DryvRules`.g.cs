using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dryv.Validation;

namespace Dryv.Rules
{
    partial class DryvRules<TModel>
    {

		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, bool>> rule)
        {
			this.Disable(rule,
				new[] { property1, },
				null
				);
			return this;
        }
		
		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, Task<bool>>> rule)
        {
			this.Disable(rule,
				new[] { property1, },
				null
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, bool>> rule)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				null
				);
			return this;
        }
		
		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, Task<bool>>> rule)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				null
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, bool>> rule)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				null
				);
			return this;
        }
		
		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, Task<bool>>> rule)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				null
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, bool>> rule)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				null
				);
			return this;
        }
		
		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, Task<bool>>> rule)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				null
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, bool>> rule)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				null
				);
			return this;
        }
		
		public DryvRules<TModel> DisableRules(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, Task<bool>>> rule)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				null
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> Rule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, DryvValidationResult>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> ClientRule(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				null,
				settings
				);
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, bool>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, Task<bool>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, bool>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, Task<bool>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, bool>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, Task<bool>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, bool>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, Task<bool>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, bool>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, Task<bool>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, DryvValidationResult>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, bool>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, bool>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, bool>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, bool>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, bool>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		
		public DryvRules<TModel> DisableRules<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<bool>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null)
        {
			this.Disable(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> Rule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.Add(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, DryvValidationResult>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
		public DryvRules<TModel> ClientRule<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5>(
			Expression<Func<TModel, object>> property1,
			Expression<Func<TModel, object>> property2,
			Expression<Func<TModel, object>> property3,
			Expression<Func<TModel, object>> property4,
			Expression<Func<TModel, object>> property5,
			Expression<Func<TModel, TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<DryvValidationResult>>> rule,
         			Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, bool> ruleSwitch = null, DryvRuleSettings settings = null)
        {
			this.AddClient(rule,
				new[] { property1, property2, property3, property4, property5, },
				ruleSwitch,
				settings
				, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }

	}
}
