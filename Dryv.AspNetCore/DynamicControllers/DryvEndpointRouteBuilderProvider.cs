using Microsoft.AspNetCore.Routing;

namespace Dryv.DynamicControllers
{
    internal class DryvEndpointRouteBuilderProvider
    {
        public IEndpointRouteBuilder RouteBuilder { get; internal set; }
    }
}