using System;
using System.Linq;
using System.Reflection;
using Dryv.Rules;
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

        public void Register(Assembly assembly, string action, DryvCompiledRule rule)
        {
            var assemblyPart = new AssemblyPart(assembly);
            this.partManager.ApplicationParts.Add(assemblyPart);

            if (this.options.Value.GetEndpoint != null)
            {
                foreach (var type in from t in assembly.DefinedTypes
                                     where typeof(Controller).IsAssignableFrom(t)
                                     select t)
                {
                    this.MapEndpoint(type, action, rule);
                }
            }

            this.actionDescriptorChangeProvider.HasChanged = true;
            this.actionDescriptorChangeProvider.TokenSource?.Cancel();
        }

        private void MapEndpoint(Type controllerTyp, string action, DryvCompiledRule rule)
        {
            var context = new DryvControllerGenerationContext(controllerTyp, action, rule);
            this.options.Value.GetEndpoint(context, this.routeBuilderProvider.RouteBuilder);
        }
    }
}