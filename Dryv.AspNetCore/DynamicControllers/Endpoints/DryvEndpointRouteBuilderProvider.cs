using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore.DynamicControllers.Endpoints
{
    internal class DryvEndpointRouteBuilderProvider
    {
        public IEndpointRouteBuilder RouteBuilder { get; internal set; }
    }
}