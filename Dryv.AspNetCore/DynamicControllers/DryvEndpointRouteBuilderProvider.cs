using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore.DynamicControllers
{
    internal class DryvEndpointRouteBuilderProvider
    {
        public IEndpointRouteBuilder RouteBuilder { get; internal set; }
    }
}