using System;
using System.Collections.Generic;
using System.Reflection;
using Dryv.AspNetCore.DynamicControllers.Translation;
using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore.DynamicControllers
{
    public static class DryvDynamicControllerOptionsExtensions
    {
        public static void MapEndpoint(this DryvDynamicControllerOptions options, Action<IEndpointRouteBuilder, Type, MethodInfo> mapper)
        {
            options.MapEndpoint = mapper;
        }

        public static void MapTemplate(this DryvDynamicControllerOptions options, Func<string, string, IDictionary<string, Type>, string> template)
        {
            options.MapTemplate = template;
        }

        public static void AddDefaultAttribute<T>(this DryvDynamicControllerOptions options, params object[] arguments)
        {
            options.DefaultAttributes[typeof(T)] = arguments;
        }
        public static void UseControllerCallWriter<T>(this DryvDynamicControllerOptions options) where T : IDryvClientServerCallWriter
        {
            options.DynamicControllerCallWriterType = typeof(T);
        }
    }
}