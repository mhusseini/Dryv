using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

namespace Dryv
{
    internal static class HttpContextExtensions
    {
        public static ClientRules GetDryvClientRules(this HttpContext httpContext, Type objectType, string propertyName)
        {
            var typeRules = httpContext.GetDryvClientRules(objectType);
            return typeRules.GetOrAdd(propertyName, p => new ClientRules
            {
                Content = RulesHelper.GetClientRulesForProperty(objectType, p),
                Name = $"dryv{Math.Abs($"{objectType.AssemblyQualifiedName}|{p}".GetHashCode())}"
            });
        }

        public static ConcurrentDictionary<string, ClientRules> GetDryvClientRules(this HttpContext httpContext, Type objectType)
        {
            var allRules = httpContext.GetDryvClientRules();
            return allRules.GetOrAdd(objectType, t => new ConcurrentDictionary<string, ClientRules>());
        }

        public static ConcurrentDictionary<Type, ConcurrentDictionary<string, ClientRules>> GetDryvClientRules(this HttpContext httpContext)
        {
            if (!httpContext.Items.TryGetValue("DryvRules", out var o) ||
                !(o is ConcurrentDictionary<Type, ConcurrentDictionary<string, ClientRules>> allRules))
            {
                allRules = new ConcurrentDictionary<Type, ConcurrentDictionary<string, ClientRules>>();
            }

            return allRules;
        }
    }
}