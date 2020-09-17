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
using Dryv.Rules;
using Dryv.Translation;
using Dryv.Translation.Visitors;

namespace Dryv.RuleDetection
{
    public class DryvRuleFinder
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
        private readonly DryvCompiler compiler;
        private readonly DryvOptions options;
        private readonly ITranslator translator;
        private readonly IReadOnlyCollection<IDryvRuleAnnotator> annotators;
        private readonly ModelTreeBuilder treeBuilder;

        public DryvRuleFinder(ModelTreeBuilder treeBuilder, DryvCompiler compiler, ITranslator translator, IReadOnlyCollection<IDryvRuleAnnotator> annotators, DryvOptions options)
        {
            this.compiler = compiler;
            this.treeBuilder = treeBuilder;
            this.translator = translator;
            this.annotators = annotators ?? new ArraySegment<IDryvRuleAnnotator>();
            this.options = options;
        }

        public IEnumerable<DryvCompiledRule> FindValidationRulesInTree(Type rootType, RuleType ruleType)
        {
            return CompiledRuleTreeCache.GetOrAdd($"{rootType.FullName}|{ruleType}", _ =>
            {
                var result = new List<DryvCompiledRule>();
                var tree = this.treeBuilder.Build(rootType);
                var flatTree = tree.Iterate(t => t.Children.Select(c => c.Child).Where(c => c != null)).ToList();
                var rules = FindRulesInModelTree(rootType, ruleType, new HashSet<Type>()).ToList();

                foreach (var rule in rules)
                {
                    foreach (var annotator in this.annotators)
                    {
                        annotator.Annotate(rule, rule.ValidationExpression);
                    }

                    var nodes = GetNodesForRule(flatTree, rule);
                    result.AddRange(nodes.Select(node => this.ApplyRuleToNode(node, rule)));
                }

                return result;
            });
        }

        private static IEnumerable<DryvCompiledRule> FindRulesInModelTree(Type rootType, RuleType ruleType, ICollection<Type> processed)
        {
            if (processed.Contains(rootType))
            {
                yield break;
            }

            processed.Add(rootType);

            var baseTypes = rootType.Iterate(t => t?.GetBaseType())
                .Where(t => t != null && t.Namespace != typeof(object).Namespace)
                .ToList();

            foreach (var rule in from type in baseTypes
                from rule in FindRulesOnType(type, ruleType)
                select rule)
            {
                yield return rule;
            }

            foreach (var rule in from type in baseTypes
                from attribute in type.GetTypeInfo().GetCustomAttributes<DryvValidationAttribute>()
                where attribute.RuleContainerType != null
                from rule in FindRulesInModelTree(attribute.RuleContainerType, ruleType, processed)
                select rule)
            {
                yield return rule;
            }

            foreach (var rule in from property in rootType.GetProperties(BindingFlagsForProperties)
                where property.PropertyType.Namespace != typeof(object).Namespace
                from rule in FindRulesInModelTree(property.PropertyType, ruleType, processed)
                select rule)
            {
                yield return rule;
            }
        }

        private static IEnumerable<DryvCompiledRule> FindRulesOnType(Type type, RuleType ruleType)
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

        private static string GetEffectiveModelPath(string originalPath, string transposedPath, PropertyInfo property)
        {
            var basePath = string.IsNullOrWhiteSpace(transposedPath) ? originalPath : transposedPath;
            var parts = basePath.Split('.');
            var path = string.Join(".", parts.Skip(1).Select(p => p.ToCamelCase()));
            var sep = string.IsNullOrWhiteSpace(path) ? string.Empty : ".";

            return path + sep + property.Name.ToCamelCase();
        }

        private static IEnumerable<ModelTreeNode> GetNodesForRule(List<ModelTreeNode> flatTree, DryvCompiledRule rule)
        {
            var path = rule.UniquePath + ":" + rule.Property.PropertyType.GetNonGenericName();
            return flatTree.FindAll(n => n.UniquePath.EndsWith(path));
        }

        private static IEnumerable<DryvCompiledRule> GetRulesOfType(DryvRules rules, RuleType ruleType)
        {
            return ruleType == RuleType.Disabling ? rules.DisablingRules : rules.ValidationRules;
        }

        private static Type TransposeExpressions(ModelTreeNode node, DryvCompiledRule rule, out LambdaExpression newValidationExpression, out string transposedPath)
        {
            var firstMember = node.Hierarchy.First();
            var modelType = firstMember.DeclaringType;
            var modelParameter = Expression.Parameter(modelType, "$m");
            var modelReplacement = TransposePath(modelParameter, node, out transposedPath);

            var parameters = new List<ParameterExpression> {modelParameter};
            parameters.AddRange(rule.ValidationExpression.Parameters.Skip(1));

            if (rule.ServiceTypes == null)
            {
                rule.ServiceTypes = parameters.Skip(1).Select(p => p.Type).ToArray();
            }

            var replacer = new NodeReplacer();
            var validationBody = replacer.Replace(
                rule.ValidationExpression.Body,
                rule.ValidationExpression.Parameters.First(),
                modelReplacement);
            newValidationExpression = Expression.Lambda(validationBody, parameters);

            return modelType;
        }

        private static Expression TransposePath(ParameterExpression modelParameter, ModelTreeNode node, out string transposedPath)
        {
            Expression modelReplacement = modelParameter;
            var sb = new StringBuilder(modelParameter.Name);

            foreach (var memberInfo in node.Hierarchy.Take(node.Hierarchy.Count - 1))
            {
                sb.Append(".");
                sb.Append(memberInfo.Name.ToCamelCase());
                modelReplacement = Expression.MakeMemberAccess(modelReplacement, memberInfo);
            }

            transposedPath = sb.ToString();
            return modelReplacement;
        }

        private DryvCompiledRule ApplyRuleToNode(ModelTreeNode node, DryvCompiledRule rule)
        {
            LambdaExpression newValidationExpression;
            Type modelType;
            string transposedPath = null;

            if (node.UniquePath != rule.UniquePath)
            {
                modelType = TransposeExpressions(node, rule, out newValidationExpression, out transposedPath);
            }
            else
            {
                newValidationExpression = rule.ValidationExpression;
                modelType = rule.ModelType;
            }

            if (rule.RelatedProperties != null)
            {
                foreach (var relatedProperty in rule.RelatedProperties)
                {
                }
            }

            var transposedRule = new DryvCompiledRule
            {
                ModelType = modelType,
                CompiledEnablingExpression = this.compiler.CompileEnablingExpression(rule, rule.EnablingExpression),
                CompiledValidationExpression = this.compiler.CompileValidationExpression(rule, newValidationExpression),
                EnablingExpression = rule.EnablingExpression,
                ValidationExpression = newValidationExpression,
                EvaluationLocation = rule.EvaluationLocation,
                PropertyExpression = rule.PropertyExpression,
                ServiceTypes = rule.ServiceTypes,
                Group = rule.Group,
                Name = rule.Name,
                RuleType = rule.RuleType,
                ModelPath = GetEffectiveModelPath(rule.ModelPath, transposedPath, rule.Property),
                Property = rule.Property,
                UniquePath = rule.UniquePath,
                Parameters = rule.Parameters,
                Annotations = rule.Annotations,
                RelatedProperties = rule.RelatedProperties?.ToDictionary(
                    p => p.Key,
                    p => GetEffectiveModelPath(p.Value, transposedPath, p.Key)),
            };

            this.Translate(transposedRule, transposedRule.ValidationExpression);

            return transposedRule;
        }

        private void Translate(DryvCompiledRule rule, Expression validationExpression)
        {
            if (this.translator == null)
            {
                return;
            }

            try
            {
                var translation = this.translator.Translate(validationExpression, rule.PropertyExpression, rule);
                rule.TranslatedValidationExpression = translation.Factory;
                rule.ServiceTypes = translation.InjectedServiceTypes;
                rule.CodeTemplate = translation.CodeTemplate;
            }
            catch (DryvException ex)
            {
                if (this.options.TranslationErrorBehavior != TranslationErrorBehavior.ValidateOnServer)
                {
                    throw;
                }

                rule.TranslatedValidationExpression = null;
                rule.ServiceTypes = null;
                rule.TranslationError = ex;
            }
        }
    }
}