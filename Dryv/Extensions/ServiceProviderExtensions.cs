using System;
using System.Collections.Generic;
using Dryv.Rules;

namespace Dryv.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static object[] GetServices(this Func<Type, object> serviceProvider, DryvCompiledRule rule, IReadOnlyDictionary<List<DryvCompiledRule>, DryvParameters> parameters)
        {
            parameters.TryGetValue(rule.Parameters, out var dryvParameters);
            return GetServices(serviceProvider, rule.PreevaluationOptionTypes, dryvParameters);
        }

        public static object[] GetServices(this Func<Type, object> serviceProvider, IList<Type> types, DryvParameters parameters = null)
        {
            var result = new object[types.Count];

            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var service = type == typeof(DryvParameters)
                ? parameters
                : serviceProvider(type);

                result[i] = service ?? throw new DryvDependencyInjectionException(type);
            }

            return result;
        }
    }
}