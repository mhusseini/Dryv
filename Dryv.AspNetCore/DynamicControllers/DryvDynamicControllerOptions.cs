using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore.DynamicControllers
{
    public class DryvDynamicControllerOptions
    {
        public Type DynamicControllerCallWriterType { get; set; }
        public DryvDynamicControllerMethods HttpMethod { get; set; } = DryvDynamicControllerMethods.Post;
        internal Action<DryvControllerGenerationContext, IEndpointRouteBuilder> MapEndpoint { get; set; }
        internal Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> MapControllerFilters { get; set; }
        internal Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> MapActionFilters { get; set; }
        internal Func<DryvControllerGenerationContext, string> MapRouteTemplate { get; set; }
    }
}