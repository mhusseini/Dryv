using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dryv.AspNetCore.DynamicControllers.Translation;
using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore.DynamicControllers
{
    public static class DryvDynamicControllerOptionsExtensions
    {
        public static void UseServerCallWriter<T>(this DryvDynamicControllerOptions options) where T : IDryvClientServerCallWriter
        {
            options.DynamicControllerCallWriterType = typeof(T);
        }

        public static void WithActionFilters(this DryvDynamicControllerOptions options, Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> filters)
        {
            // FilterExpressionValidator.ValidateFilterExpressions(filters);

            options.GetActionFilters = filters;
        }

        public static void WithControllerFilters(this DryvDynamicControllerOptions options, Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> filters)
        {
            // FilterExpressionValidator.ValidateFilterExpressions(filters);

            options.GetControllerFilters = filters;
        }

        public static void WithEndpoint(this DryvDynamicControllerOptions options, Action<DryvControllerGenerationContext, IEndpointRouteBuilder> mapper)
        {
            options.GetEndpoint = mapper;
        }

        public static void WithRoute(this DryvDynamicControllerOptions options, Func<DryvControllerGenerationContext, string> template)
        {
            options.GetRoute = template;
        }
    }
}