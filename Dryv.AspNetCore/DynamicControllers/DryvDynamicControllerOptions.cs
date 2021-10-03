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

        internal Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> GetActionFilters { get; set; }
        internal Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> GetControllerFilters { get; set; }
        internal Action<DryvControllerGenerationContext, IEndpointRouteBuilder> SetEndpoint { get; set; }
        internal Func<DryvControllerGenerationContext, DryvDynamicControllerMethods> GetHttpMethod { get; set; }
        internal Func<DryvControllerGenerationContext, string> GetRoute { get; set; }
    }
}