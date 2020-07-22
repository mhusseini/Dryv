using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore.DynamicControllers
{
    public class DryvDynamicControllerOptions
    {
        public Type DynamicControllerCallWriterType { get; set; }
        public Action<Assembly> GeneratedAssemblyOutput { get; set; }
        public bool Greedy { get; set; }
        public DryvDynamicControllerMethods HttpMethod { get; set; } = DryvDynamicControllerMethods.Post;
        internal Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> MapActionFilters { get; set; }
        internal Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> MapControllerFilters { get; set; }
        internal Action<DryvControllerGenerationContext, IEndpointRouteBuilder> MapEndpoint { get; set; }
        internal Func<DryvControllerGenerationContext, string> MapRouteTemplate { get; set; }
    }
}