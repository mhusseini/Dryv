using System.Security.Cryptography;
using System.Text;
using Dryv.AspNetCore.DynamicControllers;
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

        public static string DefaultRoute(DryvControllerGenerationContext context) => $"_v/c{CreateMd5(context.ControllerFullName)}";

        private static string CreateMd5(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString());
            }
            
            return sb.ToString();
        }
    }
}