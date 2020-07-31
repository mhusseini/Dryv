using System;
using Dryv.AspNetCore.DynamicControllers;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.AspNetCore.DynamicControllers.Translation;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore
{
    public static class DryvDynamicControllerBuilderExtensions
    {
        public static IDryvMvcBuilder AddDryvDynamicControllers(this IDryvMvcBuilder dryvBuilder, Action<DryvDynamicControllerOptions> setupAction = null)
        {
            dryvBuilder.Options.Translators.Add<DryvDynamicControllerTranslator>();
            dryvBuilder.Options.Translators.Add<DryvDynamicControllerMethodTranslator>();

            var options = new DryvDynamicControllerOptions();
            setupAction?.Invoke(options);

            var services = dryvBuilder.MvcBuilder.Services;
            services.AddSingleton(Options.Create(options));
            services.AddSingleton<ControllerGenerator>();
            services.AddSingleton<DryvDynamicControllerRegistration>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddSingleton(typeof(IDryvClientServerCallWriter), options.DynamicControllerCallWriterType ?? typeof(DryvClientServerCallWriter));

            var actionDescriptorChangeProvider = new DryvDynamicActionDescriptorChangeProvider();
            services.AddSingleton<IActionDescriptorChangeProvider>(actionDescriptorChangeProvider);
            services.AddSingleton(actionDescriptorChangeProvider);

            SetupEndpoints(services);

            return dryvBuilder;
        }

        private static void SetupEndpoints(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mvcOptions = serviceProvider.GetService<IOptions<MvcOptions>>().Value;
            var dynamicControllerOptions = serviceProvider.GetService<IOptions<DryvDynamicControllerOptions>>().Value;

            if (mvcOptions.EnableEndpointRouting && dynamicControllerOptions.GetEndpoint == null && dynamicControllerOptions.GetRoute != null)
            {
                throw new DryvConfigurationException("When MvcOptions.EnableEndpointRouting is true, DryvDynamicControllerOptions.MapRouteTemplate cannot be used. Instead, please specify DryvDynamicControllerOptions.MapEndpoint or leave DryvDynamicControllerOptions.MapTemplate and DryvDynamicControllerOptions.MapEndpoint empty for default values.");
            }

            if (!mvcOptions.EnableEndpointRouting && dynamicControllerOptions.GetEndpoint != null && dynamicControllerOptions.GetRoute == null)
            {
                throw new DryvConfigurationException("When MvcOptions.EnableEndpointRouting is false, DryvDynamicControllerOptions.MapEndpoint cannot be used. Instead, please specify DryvDynamicControllerOptions.MapRouteTemplate or leave DryvDynamicControllerOptions.MapTemplate and DryvDynamicControllerOptions.MapEndpoint empty for default values.");
            }

            if (mvcOptions.EnableEndpointRouting && dynamicControllerOptions.GetEndpoint == null)
            {
                dynamicControllerOptions.GetEndpoint = CustomizationDefaults.DefaultEndpoint;
            }
            else if (!mvcOptions.EnableEndpointRouting && dynamicControllerOptions.GetRoute == null)
            {
                dynamicControllerOptions.GetRoute = CustomizationDefaults.DefaultRoute;
            }

            if (dynamicControllerOptions.GetHttpMethod == null)
            {
                dynamicControllerOptions.GetHttpMethod = CustomizationDefaults.DefaultHttpMethod;
            }
        }
    }
}