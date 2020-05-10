using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore.DynamicControllers
{
    public class DryvDynamicControllerOptions
    {
        public Type DynamicControllerCallWriterType { get; set; }
        public DryvDynamicControllerMethods HttpMethod { get; set; } = DryvDynamicControllerMethods.Post;
        internal Dictionary<Type, object[]> DefaultAttributes { get; } = new Dictionary<Type, object[]>();
        internal Action<IEndpointRouteBuilder, Type, MethodInfo> MapEndpoint { get; set; }
        internal Func<string, string, IDictionary<string, Type>, string> MapTemplate { get; set; }
    }
}