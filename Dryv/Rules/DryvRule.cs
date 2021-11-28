using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv.Rules
{
    public class DryvRule
    {
        public DryvEvaluationLocation EvaluationLocation { get; set; }
        public List<MemberExpression> Properties { get; set; } = new List<MemberExpression>();
        
        public Dictionary<string, object> Annotations { get; set; } = new Dictionary<string, object>();

        public string Name { get; set; }

        public string Group { get; set; }

        public Delegate ValidationFunction { get; set; }

        public Delegate AsyncValidationFunction { get; set; }

        public Expression ValidationFunctionExpression { get; set; }

        public Expression AsyncValidationFunctionExression { get; set; }
    }
}