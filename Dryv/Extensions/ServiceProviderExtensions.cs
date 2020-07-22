using System;
using System.Collections.Generic;

namespace Dryv.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static object[] GetServices(this Func<Type, object> serviceProvider, IList<Type> types)
        {
            var result = new object[types.Count];

            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var service = serviceProvider(type);

                result[i] = service ?? throw new DryvDependencyInjectionException(type);
            }

            return result;
        }
    }
}