using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Extensions;
using Dryv.Rules;

namespace Dryv
{
    public class DryvParametersHelper
    {
        public static Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters> GetDryvParameters(IEnumerable<DryvCompiledRule> rules, Func<Type, object> serviceProvider)
        {
            return rules
                .Select(r => r.Parameters)
                .Distinct()
                .ToDictionary(ps => ps, ps => new DryvParameters(ps.ToDictionary(p => p.Name, p =>
                {
                    var services = serviceProvider.GetServices(p.PreevaluationOptionTypes);
                    return p.CompiledValidationExpression(null, services);
                })));
        }
    }
}