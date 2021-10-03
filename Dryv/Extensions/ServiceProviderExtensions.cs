﻿using System;
using System.Collections.Generic;
using Dryv.Rules;

namespace Dryv.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static T GetService<T>(this Func<Type, object> serviceProvider) where T : class
        {
            var o = serviceProvider(typeof(T));
            return o as T;
        }
        
        public static object[] GetServices(this Func<Type, object> serviceProvider, DryvCompiledRule rule, IReadOnlyDictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters> parameters)
        {
            parameters.TryGetValue(rule.Parameters, out var dryvParameters);
            return GetServices(serviceProvider, rule.ServiceTypes, dryvParameters);
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