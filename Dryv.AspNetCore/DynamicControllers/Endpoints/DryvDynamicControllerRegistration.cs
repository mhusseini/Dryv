using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.Endpoints
{
    internal class DryvDynamicControllerRegistration
    {
        private readonly DryvDynamicActionDescriptorChangeProvider actionDescriptorChangeProvider;
        private readonly IOptions<DryvDynamicControllerOptions> options;
        private readonly ApplicationPartManager partManager;
        private readonly DryvEndpointRouteBuilderProvider routeBuilderProvider;

        public DryvDynamicControllerRegistration(ApplicationPartManager partManager, DryvEndpointRouteBuilderProvider routeBuilderProvider, DryvDynamicActionDescriptorChangeProvider actionDescriptorChangeProvider, IOptions<DryvDynamicControllerOptions> options)
        {
            this.routeBuilderProvider = routeBuilderProvider;
            this.partManager = partManager;
            this.actionDescriptorChangeProvider = actionDescriptorChangeProvider;
            this.options = options;
        }

        public void Register(Assembly assembly, MethodInfo method)
        {
            var assemblyPart = new AssemblyPart(assembly);
            this.partManager.ApplicationParts.Add(assemblyPart);

            if (this.options.Value.MapEndpoint != null)
            {
                foreach (var type in from t in assembly.DefinedTypes
                                     where typeof(Controller).IsAssignableFrom(t)
                                     select t)
                {
                    this.MapEndpoint(type, method);
                }
            }

            this.actionDescriptorChangeProvider.HasChanged = true;
            this.actionDescriptorChangeProvider.TokenSource.Cancel();
        }

        private void MapEndpoint(Type controllerTyp, MethodInfo method)
        {
            var context = new DryvControllerGenerationContext(controllerTyp, method);
            this.options.Value.MapEndpoint(context, this.routeBuilderProvider.RouteBuilder);
        }
    }
}