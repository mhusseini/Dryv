using System;
using System.Collections.Generic;
using System.Reflection;
using Dryv.AspNetCore.DynamicControllers;
using Dryv.AspNetCore.DynamicControllers.Translation;
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
    public static class DryvDynamicControllerMvcBuilderExtensions
    {
        public static IMvcBuilder AddDryvDynamicControllers(this IMvcBuilder mvcBuilder, Action<DryvDynamicControllerOptions> setupAction = null)
        {
            var services = mvcBuilder.Services;
            var options = new DryvDynamicControllerOptions();

            setupAction?.Invoke(options);

            services.AddSingleton(Options.Create(options));
            services.AddSingleton<DryvDynamicDelegatingControllerGenerator>();
            services.AddSingleton<DryvDynamicControllerRegistration>();
            services.AddSingleton<DryvDynamicControllerClientCodeModifier>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<ICustomTranslator, DryvDynamicControllerTranslator>();

            if (options.DynamicControllerCallWriterType != null)
            {
                services.TryAddSingleton(typeof(IDryvDynamicControllerCallWriter), options.DynamicControllerCallWriterType);
            }
            else
            {
                services.TryAddSingleton<IDryvDynamicControllerCallWriter, DefaultDryvDynamicControllerCallWriter>();
            }

            var actionDescriptorChangeProvider = new DryvDynamicActionDescriptorChangeProvider();
            services.AddSingleton<IActionDescriptorChangeProvider>(actionDescriptorChangeProvider);
            services.AddSingleton(actionDescriptorChangeProvider);

            SetupEndpointMApping(services);

            return mvcBuilder;
        }

        private static void DefaultEndpointMapping(IEndpointRouteBuilder builder, Type type, MethodInfo method)
        {
            var controller = type.Name.Replace("Controller", string.Empty);
            builder.MapControllerRoute(type.FullName, $"validation/{controller}/{method.Name}", new { controller, action = method.Name });
        }

        private static string DefaultTemplateMapping(string controller, string action, IDictionary<string, Type> arg3)
        {
            return $"validation/{controller.Replace("Controller", string.Empty)}/{action}";
        }

        private static void SetupEndpointMApping(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mvcOptions = serviceProvider.GetService<IOptions<MvcOptions>>().Value;
            var dynamicControllerOptions = serviceProvider.GetService<IOptions<DryvDynamicControllerOptions>>().Value;

            if (mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapEndpoint == null && dynamicControllerOptions.MapTemplate != null)
            {
                throw new DryvConfigurationException("When MvcOptions.EnableEndpointRouting is true, DryvDynamicControllerOptions.MapTemplate cannot be used. Instead, please specify DryvDynamicControllerOptions.MapEndpoint or leave DryvDynamicControllerOptions.MapTemplate and DryvDynamicControllerOptions.MapEndpoint empty for default values.");
            }

            if (!mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapEndpoint != null && dynamicControllerOptions.MapTemplate == null)
            {
                throw new DryvConfigurationException("When MvcOptions.EnableEndpointRouting is false, DryvDynamicControllerOptions.MapEndpoint cannot be used. Instead, please specify DryvDynamicControllerOptions.MapTemplate or leave DryvDynamicControllerOptions.MapTemplate and DryvDynamicControllerOptions.MapEndpoint empty for default values.");
            }

            if (mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapEndpoint == null)
            {
                dynamicControllerOptions.MapEndpoint = DefaultEndpointMapping;
            }
            else if (!mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapTemplate == null)
            {
                dynamicControllerOptions.MapTemplate = DefaultTemplateMapping;
            }
        }
    }
}