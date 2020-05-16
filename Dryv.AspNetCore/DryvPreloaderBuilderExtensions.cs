using System;
using Dryv.AspNetCore.Internal;
using Dryv.AspNetCore.PreLoading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore
{
    public static class DryvPreloaderBuilderExtensions
    {
        public static IDryvMvcBuilder AddDryvPreloading(this IDryvMvcBuilder dryvBuilder, Action<DryvPreloaderOptions> setupAction = null)
        {
            var options = new DryvPreloaderOptions { IsEnabled = true };
            setupAction?.Invoke(options);

            dryvBuilder.Services.AddSingleton(Options.Create(options));
            dryvBuilder.Services.AddSingleton<DryvPreloader>();
            dryvBuilder.Services.AddSingleton(new DryvMvcInitializer(services => services.GetService<DryvPreloader>().Preload()));

            return dryvBuilder;
        }
    }
}