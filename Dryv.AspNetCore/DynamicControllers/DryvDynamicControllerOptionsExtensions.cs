using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.AspNetCore.DynamicControllers.Translation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore.DynamicControllers
{
    public static class DryvDynamicControllerOptionsExtensions
    {
        public static void MapEndpoint(this DryvDynamicControllerOptions options, Action<DryvControllerGenerationContext, IEndpointRouteBuilder> mapper)
        {
            options.MapEndpoint = mapper;
        }

        public static void MapFilters(this DryvDynamicControllerOptions options, Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> filters)
        {
            FilterExpressionValidator.ValidateFilterExpressions(filters);

            options.MapFilters = filters;
        }

        public static void MapRouteTemplate(this DryvDynamicControllerOptions options, Func<DryvControllerGenerationContext, string> template)
        {
            options.MapRouteTemplate = template;
        }

        public static void UseControllerCallWriter<T>(this DryvDynamicControllerOptions options) where T : IDryvClientServerCallWriter
        {
            options.DynamicControllerCallWriterType = typeof(T);
        }
    }
}