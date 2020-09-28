using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dryv.Extensions;
using Dryv.Rules;

namespace Dryv
{
    public class DryvParametersHelper
    {
        public static async Task<Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters>> GetDryvParameters(IEnumerable<DryvCompiledRule> rules, Func<Type, object> serviceProvider)
        {
            var result = new Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters>();

            foreach (var rule in rules)
            {
                var parameterValues = new Dictionary<string, object>();

                foreach (var parameter in rule.Parameters)
                {
                    var services = serviceProvider.GetServices(parameter.ServiceTypes);
                    var possiblyAsyncValue = parameter.CompiledValidationExpression(null, services);
                    var value = await TaskValueHelper.GetPossiblyAsyncValue(possiblyAsyncValue);
                    
                    parameterValues.Add(parameter.Name, value);
                }

                result.Add(rule.Parameters, new DryvParameters(parameterValues));
            }

            return result;
        }
    }
}