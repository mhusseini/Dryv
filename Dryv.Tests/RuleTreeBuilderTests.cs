using System.Collections.Generic;
using Dryv.Rules;
using Dryv.Rules.Sources;
using Xunit;

namespace Dryv.Tests
{
    public class RuleTreeBuilderTests
    {
        [Fact]
        public void Build()
        {
            var sut = new DryvRuleTreeBuilder(new List<IDryvRuleSource> { new ReflectionDryvRuleSource() });
            var tree = sut.Build(typeof(Parent));
        }


        /*
         * .item:C.name => !"0"     2
         * .item*:C.name => !""     1
         * *:C.name => !null        3
         *
         * #root
         * .item => ref0
         *
         * #ref0
         * .name => !"0"            2
         * .name => !""             1
         * .name => !null           3
         * .item => ref1
         * 
         * #ref1
         * .name => !""             1
         * .name => !null           3
         * .item => ref1
         *
         *
         *
         * rule1: Parent, Child, name           -> global
         * rule2: Parent, Parent, item.name     -> local
         * rule3: Child, Child, name            -> global
         */

        public class Parent
        {
            private static DryvRules Rules = DryvRules.For<Child>()
                .Rule(m => m.Name, m => m.Item.Name == "" ? "not empty" : null, o => o.Name("rule1"));

            private static DryvRules Rules2 = DryvRules.For<Parent>()
                .Rule(m => m.Item.Name, m => m.Item.Name == "0" ? "not zero" : null, o => o.Name("rule2"));

            public string Name { get; set; }

            public Child Item { get; set; }
        }

        public class Child
        {
            private static DryvRules Rules = DryvRules.For<Child>()
                .Rule(m => m.Name, m => m.Name == null ? "not null" : null, o => o.Name("rule3"));

            public string Name { get; set; }

            public Child Item { get; set; }
        }
    }
}