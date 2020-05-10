using System;
using Dryv.DynamicControllers;
using Dryv.DynamicControllers.Translation;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dryv
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

            return mvcBuilder;
        }
    }
}