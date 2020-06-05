using System;
using Dryv.AspNetCore.DynamicControllers;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.AspNetCore.DynamicControllers.Translation;
using Dryv.AspNetCore.Internal;
using Dryv.Translation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore
{
    public static class DryvDynamicControllerBuilderExtensions
    {
        public static IDryvMvcBuilder AddDryvDynamicControllers(this IDryvMvcBuilder dryvBuilder, Action<DryvDynamicControllerOptions> setupAction = null)
        {
            var services = dryvBuilder.Services;
            var options = new DryvDynamicControllerOptions();

            setupAction?.Invoke(options);

            services.AddSingleton(Options.Create(options));
            services.AddSingleton<ControllerGenerator>();
            services.AddSingleton<DryvDynamicControllerRegistration>();
            services.AddSingleton<ICustomTranslator, DryvDynamicControllerTranslator>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddSingleton(typeof(IDryvClientServerCallWriter), options.DynamicControllerCallWriterType ?? typeof(DryvClientServerCallWriter));

            var actionDescriptorChangeProvider = new DryvDynamicActionDescriptorChangeProvider();
            services.AddSingleton<IActionDescriptorChangeProvider>(actionDescriptorChangeProvider);
            services.AddSingleton(actionDescriptorChangeProvider);

            SetupEndpointMapping(services);

            return dryvBuilder;
        }

        private static void DefaultEndpointMapping(DryvControllerGenerationContext context, IEndpointRouteBuilder builder)
        {
            builder.MapControllerRoute(
                context.ControllerFullName,
                $"validation/{context.Controller}/{context.Action}",
                new { controller = context.Controller, action = context.Action });
        }

        private static string DefaultTemplateMapping(DryvControllerGenerationContext context)
        {
            return $"validation/{context.Controller}/{context.Action}";
        }

        private static void SetupEndpointMapping(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mvcOptions = serviceProvider.GetService<IOptions<MvcOptions>>().Value;
            var dynamicControllerOptions = serviceProvider.GetService<IOptions<DryvDynamicControllerOptions>>().Value;

            if (mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapEndpoint == null && dynamicControllerOptions.MapRouteTemplate != null)
            {
                throw new DryvConfigurationException("When MvcOptions.EnableEndpointRouting is true, DryvDynamicControllerOptions.MapRouteTemplate cannot be used. Instead, please specify DryvDynamicControllerOptions.MapEndpoint or leave DryvDynamicControllerOptions.MapTemplate and DryvDynamicControllerOptions.MapEndpoint empty for default values.");
            }

            if (!mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapEndpoint != null && dynamicControllerOptions.MapRouteTemplate == null)
            {
                throw new DryvConfigurationException("When MvcOptions.EnableEndpointRouting is false, DryvDynamicControllerOptions.MapEndpoint cannot be used. Instead, please specify DryvDynamicControllerOptions.MapRouteTemplate or leave DryvDynamicControllerOptions.MapTemplate and DryvDynamicControllerOptions.MapEndpoint empty for default values.");
            }

            if (mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapEndpoint == null)
            {
                dynamicControllerOptions.MapEndpoint = DefaultEndpointMapping;
            }
            else if (!mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapRouteTemplate == null)
            {
                dynamicControllerOptions.MapRouteTemplate = DefaultTemplateMapping;
            }
        }
    }
}