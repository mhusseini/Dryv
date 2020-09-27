using Dryv.AspNetCore.DynamicControllers;
using Dryv.AspNetCore.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore
{
    public static class CustomizationDefaults
    {
        public static void DefaultEndpoint(DryvControllerGenerationContext context, IEndpointRouteBuilder builder) =>
            builder.MapControllerRoute(
                context.ControllerFullName,
                DefaultRoute(context),
                new { controller = context.Controller, action = context.Action });

        public static DryvDynamicControllerMethods DefaultHttpMethod(DryvControllerGenerationContext context) => DryvDynamicControllerMethods.Post;

        public static string DefaultRoute(DryvControllerGenerationContext context) => $"_v/c{Md5Helper.GetShortMd5(context.ControllerFullName, 8)}";
    }
}