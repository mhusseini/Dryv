using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Dryv.Tests
{
    public class RulesBuilderTests
    {
        [Fact]
        public void AddSimpleRule_ExpectRuleWithProperty()
        {
            var rules = DryvRules.For<Model>()
                .Rule(m => m.Name);

            Assert.Single(rules.ValidationRules);
            Assert.Collection(rules.ValidationRules, rule =>
            {
                Assert.Single(rule.Properties);
            });
        }

        [Fact]
        public void AddRule_ExpectClientAndServerEvaluationLocation()
        {
            var rules = DryvRules.For<Model>()
                .Rule(m => m.Name);

            Assert.Collection(rules.ValidationRules, rule =>
            {
                Assert.Equal(DryvEvaluationLocation.Client | DryvEvaluationLocation.Server, rule.EvaluationLocation);
            });
        }

        [Fact]
        public void AddServerRule_ExpecterverEvaluationLocation()
        {
            var rules = DryvRules.For<Model>()
                .ServerRule(m => m.Name);

            Assert.Collection(rules.ValidationRules, rule =>
            {
                Assert.Equal(DryvEvaluationLocation.Server, rule.EvaluationLocation);
            });
        }

        [Fact]
        public void AddClientRule_ExpecterverEvaluationLocation()
        {
            var rules = DryvRules.For<Model>()
                .ClientRule(m => m.Name);

            Assert.Collection(rules.ValidationRules, rule =>
            {
                Assert.Equal(DryvEvaluationLocation.Client, rule.EvaluationLocation);
            });
        }

        [Fact]
        public void AddRuleWithExpression_ExpectValidationFunctionExpression()
        {
            var rules = DryvRules.For<Model>()
                .Rule(m => m.Name, m => m.Name == null ? null : "error");

            Assert.Collection(rules.ValidationRules, rule =>
            {
                Assert.Equal(ExpressionType.Lambda, rule.ValidationFunctionExpression.NodeType);
            });
        }

        [Fact]
        public void AddRuleAddProperty_ExpectTwoProperties()
        {
            var rules = DryvRules.For<Model>()
                .Rule(m => m.Name, config => config
                    .Property(m => m.Address));

            Assert.Collection(rules.ValidationRules, rule =>
            {
                Assert.Equal(2, rule.Properties.Count);
            });
        }

        [Fact]
        public void AddRuleAddDuplicateProperty_ExpectDistinctProperties()
        {
            var rules = DryvRules.For<Model>()
                .Rule(m => m.Name, config => config
                    .Property(m => m.Address)
                    .Property(m => m.Address));

            Assert.Collection(rules.ValidationRules, rule =>
            {
                Assert.Equal(2, rule.Properties.Count);
                var names = rule.Properties.Select(p => p.Member.Name).ToList();
                Assert.Contains(nameof(Model.Name), names);
                Assert.Contains(nameof(Model.Address), names);
            });
        }

        [Fact]
        public void AddRuleAddGroup_ExpectGroup()
        {
            var rules = DryvRules.For<Model>()
                .Rule(m => m.Name, config => config
                    .Group("group"));

            Assert.Collection(rules.ValidationRules, rule =>
            {
                Assert.Equal("group", rule.Group);
            });
        }

        [Fact]
        public void AddRuleAddAnnotation_ExpectAnnontation()
        {
            var rules = DryvRules.For<Model>()
                .Rule(m => m.Name, config => config
                    .Annotation("key1", "value1"));

            Assert.Collection(rules.ValidationRules, rule =>
            {
                Assert.True(rule.Annotations.ContainsKey("key1"));
                Assert.Equal("value1", rule.Annotations["key1"]);
            });
        }

        public class Model
        {
            public string Name { get; set; }

            public string Address { get; set; }

            public string Phone { get; set; }
        }
    }
}