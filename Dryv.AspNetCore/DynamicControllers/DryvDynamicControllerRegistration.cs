using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;

namespace Dryv.DynamicControllers
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

        public void Register(Assembly assembly)
        {
            var assemblyPart = new AssemblyPart(assembly);
            this.partManager.ApplicationParts.Add(assemblyPart);

            if (this.options.Value.MapEndpoint != null)
            {
                foreach (var type in from t in assembly.DefinedTypes
                                     where typeof(Controller).IsAssignableFrom(t)
                                     select t)
                {
                    this.MapEndpoint(type);
                }
            }

            this.actionDescriptorChangeProvider.HasChanged = true;
            this.actionDescriptorChangeProvider.TokenSource.Cancel();
        }

        private void MapEndpoint(Type controllerTyp)
        {
            var m = controllerTyp.GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();
            this.options.Value.MapEndpoint(this.routeBuilderProvider.RouteBuilder, controllerTyp, m);
        }
    }
}