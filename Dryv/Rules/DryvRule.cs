using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Dryv.Rules
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class DryvRule
    {
        public DryvEvaluationLocation EvaluationLocation { get; set; }

        public List<DryvRuleProperty> Properties { get; set; } = new List<DryvRuleProperty>();

        public Dictionary<string, object> Annotations { get; set; } = new Dictionary<string, object>();

        public DryvRuleType RuleType { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public Type ModelType { get; set; }

        public Delegate ValidationFunction { get; set; }

        public Delegate AsyncValidationFunction { get; set; }

        public Expression ValidationFunctionExpression { get; set; }

        public Expression AsyncValidationFunctionExression { get; set; }

        public string RuleSetName { get; set; }

        public Type DeclaringType { get; set; }
    }
}