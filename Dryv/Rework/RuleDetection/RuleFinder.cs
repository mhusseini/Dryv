using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Reflection;
using Dryv.Rework.Compilation;
using Dryv.RuleDetection;
using Dryv.Rules;
using Dryv.Translation;

namespace Dryv.Rework.RuleDetection
{
    internal class RuleFinder
    {
        private const BindingFlags BindingFlagsForProperties = BindingFlags.FlattenHierarchy |
                                                               BindingFlags.Instance |
                                                               BindingFlags.Public |
                                                               BindingFlags.NonPublic;

        private const BindingFlags BindingFlagsForRules = BindingFlags.Static |
                                                          BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.FlattenHierarchy;

        private static readonly ConcurrentDictionary<string, IEnumerable<DryvCompiledRule>> CompiledRuleCache = new ConcurrentDictionary<string, IEnumerable<DryvCompiledRule>>();
        private static readonly ConcurrentDictionary<string, IEnumerable<DryvCompiledRule>> CompiledRuleTreeCache = new ConcurrentDictionary<string, IEnumerable<DryvCompiledRule>>();
        private readonly RuleCompiler compiler;
        private readonly ModelTreeBuilder treeBuilder;
        private readonly DryvOptions options;
        private readonly ITranslator translator;

        public RuleFinder(ModelTreeBuilder treeBuilder, RuleCompiler compiler, ITranslator translator, DryvOptions options)
        {
            this.compiler = compiler;
            this.treeBuilder = treeBuilder;
            this.translator = translator;
            this.options = options;
        }

        public IEnumerable<DryvCompiledRule> FindValidationRulesInTree(Type rootType, RuleType ruleType)
        {
            return CompiledRuleTreeCache.GetOrAdd($"{rootType.FullName}|{ruleType}", _ =>
            {
                var result = new List<DryvCompiledRule>();
                var tree = this.treeBuilder.Build(rootType);
                var flatTree = tree.Iterate(t => t.Children.Select(c => c.Child).Where(c => c != null)).ToList();
                var rules = FindValidationRulesInTree(rootType, ruleType, new HashSet<Type>()).ToList();

                foreach (var rule in rules)
                {
                    var nodes = flatTree.FindAll(n => n.UniquePath.EndsWith(rule.UniquePath));

                    foreach (var node in nodes)
                    {
                        LambdaExpression newValidationExpression;
                        LambdaExpression newEnablingExpression;
                        string transposedPath = null;

                        if (node.UniquePath != rule.UniquePath)
                        {
                            var firstMember = node.Hierarchy.First();
                            var parameter = Expression.Parameter(firstMember.DeclaringType, "m");
                            Expression expression = parameter;
                            var sb = new StringBuilder(parameter.Name);

                            foreach (var memberInfo in node.Hierarchy)
                            {
                                sb.Append(".");
                                sb.Append(memberInfo.Name.ToCamelCase());
                                expression = Expression.MakeMemberAccess(expression, memberInfo);
                            }

                            transposedPath = sb.ToString();

                            newValidationExpression = Expression.Lambda(Expression.Invoke(rule.ValidationExpression, expression), parameter);
                            newEnablingExpression = rule.EnablingExpression != null
                                ? Expression.Lambda(Expression.Invoke(rule.EnablingExpression, expression), parameter)
                                : null;
                        }
                        else
                        {
                            newValidationExpression = rule.ValidationExpression;
                            newEnablingExpression = rule.EnablingExpression;
                        }

                        var transposedRule = new DryvCompiledRule
                        {
                            CompiledEnablingExpression = this.compiler.CompileEnablingExpression(rule, newEnablingExpression),
                            CompiledValidationExpression = this.compiler.CompileValidationExpression(rule, newValidationExpression),
                            EnablingExpression = rule.EnablingExpression,
                            EvaluationLocation = rule.EvaluationLocation,
                            PropertyExpression = rule.PropertyExpression,
                            ValidationExpression = rule.ValidationExpression,
                            GroupName = rule.GroupName,
                            IsDisablingRule = rule.IsDisablingRule,
                            ModelPath = GetEffectiveModelPath(rule.ModelPath, transposedPath, rule.Property),
                            TransposedPath = transposedPath,
                            ModelType = rule.ModelType,
                            Property = rule.Property,
                            UniquePath = rule.UniquePath,
                        };

                        this.Translate(transposedRule, rule.ValidationExpression);

                        result.Add(transposedRule);
                    }
                }

                return result;
            });
        }


        private static string GetEffectiveModelPath(string originalPath, string transposedPath, PropertyInfo property)
        {
            if (string.IsNullOrWhiteSpace(transposedPath))
            {
                return originalPath;
            }

            var parts = transposedPath.Split('.');
            var path = string.Join(".", parts.Skip(1).Select(p => p.ToCamelCase()));

            return path + "." + property.Name.ToCamelCase();
        }
        private static IEnumerable<DryvCompiledRule> FindValidationRulesInTree(Type rootType, RuleType ruleType, ICollection<Type> processed)
        {
            if (processed.Contains(rootType))
            {
                yield break;
            }

            processed.Add(rootType);

            var baseTypes = rootType
                .Iterate(t => t.GetBaseType())
                .Where(t => t.Namespace != typeof(object).Namespace)
                .ToList();

            foreach (var rule in from type in baseTypes
                                 from rule in FindValidationRulesOnType(type, ruleType)
                                 select rule)
            {
                yield return rule;
            }

            foreach (var rule in from type in baseTypes
                                 from attribute in type.GetTypeInfo().GetCustomAttributes<DryvValidationAttribute>()
                                 from rule in FindValidationRulesInTree(attribute.RuleContainerType, ruleType, processed)
                                 select rule)
            {
                yield return rule;
            }

            foreach (var rule in from property in rootType.GetProperties(BindingFlagsForProperties)
                                 where property.PropertyType.Namespace != typeof(object).Namespace
                                 from rule in FindValidationRulesInTree(property.PropertyType, ruleType, processed)
                                 select rule)
            {
                yield return rule;
            }
        }

        private static IEnumerable<DryvCompiledRule> FindValidationRulesOnType(Type type, RuleType ruleType)
        {
            return CompiledRuleCache.GetOrAdd($"{type.FullName}|{ruleType}", _ =>
            {
                var typeInfo = type.GetTypeInfo();

                var fromFields = from p in typeInfo.GetFields(BindingFlagsForRules)
                                 where typeof(DryvRules).IsAssignableFrom(p.FieldType)
                                 select p.GetValue(null) as DryvRules;

                var fromProperties = from p in typeInfo.GetProperties(BindingFlagsForRules)
                                     where typeof(DryvRules).IsAssignableFrom(p.PropertyType)
                                     select p.GetValue(null) as DryvRules;

                var fromMethods = from m in typeInfo.GetMethods(BindingFlagsForRules)
                                  where m.IsStatic
                                        && !m.GetParameters().Any()
                                        && typeof(DryvRules).IsAssignableFrom(m.ReturnType)
                                        && !m.ContainsGenericParameters
                                  select m.Invoke(null, null) as DryvRules;

                return (from rules in fromFields.Union(fromProperties).Union(fromMethods)
                        from rule in GetRulesOfType(rules, ruleType)
                        select rule).ToList();
            });
        }

        private static IEnumerable<DryvCompiledRule> GetRulesOfType(DryvRules rules, RuleType ruleType)
        {
            return ruleType switch
            {
                RuleType.Disabling => rules.DisablingRules,
                _ => rules.ValidationRules
            };
        }

        private void Translate(DryvCompiledRule rule, Expression validationExpression)
        {
            try
            {
                var translation = translator.Translate(validationExpression, rule.PropertyExpression, rule);
                rule.TranslatedValidationExpression = translation.Factory;
                rule.PreevaluationOptionTypes = translation.OptionTypes;
                rule.CodeTemplate = translation.CodeTemplate;
            }
            catch (DryvException ex)
            {
                switch (this.options.TranslationErrorBehavior)
                {
                    case TranslationErrorBehavior.ValidateOnServer:
                        rule.TranslatedValidationExpression = null;
                        rule.PreevaluationOptionTypes = null;
                        rule.TranslationError = ex;
                        break;

                    default:
                        throw;
                }
            }
        }
    }
}