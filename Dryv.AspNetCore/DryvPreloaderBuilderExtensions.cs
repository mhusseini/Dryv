using System;
using Dryv.AspNetCore.Internal;
using Dryv.AspNetCore.PreLoading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore
{
    public static class DryvPreloaderBuilderExtensions
    {
        public static IDryvMvcBuilder AddDryvPreloading(this IDryvMvcBuilder mvcBuilder, Action<DryvPreloaderOptions> setupAction = null)
        {
            var options = new DryvPreloaderOptions { IsEnabled = true };
            setupAction?.Invoke(options);

            mvcBuilder.MvcBuilder.Services.AddSingleton(Options.Create(options));
            mvcBuilder.MvcBuilder.Services.AddSingleton<DryvPreloader>();
            mvcBuilder.MvcBuilder.Services.AddSingleton(new DryvMvcInitializer(services => services.GetService<DryvPreloader>().Preload()));

            return mvcBuilder;
        }
    }
}