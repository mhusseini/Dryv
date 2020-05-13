using System;
using Dryv.AspNetCore.DynamicControllers;
using Dryv.AspNetCore.Internal;
using Dryv.Compilation;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.Translation;
using Dryv.Translation.Translators;
using Dryv.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore
{
    public static class DryvMvcBuilderExtensions
    {
        public static IDryvMvcBuilder AddDryv(this IMvcBuilder mvcBuilder, Action<DryvOptions> setupAction = null)
        {
            var options = new DryvOptions();
            setupAction?.Invoke(options);

            mvcBuilder.AddMvcOptions(opts =>
            {
                // As long as mvc validation is not async, we'll
                // run the async validation from an action attribute.
                opts.Filters.Add<DryvValidationFilterAttribute>();
                opts.ModelValidatorProviders.Add(new DryvModelValidatorProvider());
            });

            return RegsterServices(mvcBuilder.Services, options);
        }

        private static IDryvMvcBuilder RegsterServices(this IServiceCollection services, DryvOptions options)
        {
            services.TryAddSingleton<DryvClientValidationLoader>();
            services.TryAddSingleton<DryvEndpointRouteBuilderProvider>();
            services.TryAddSingleton<IDryvClientValidationProvider, DryvClientValidationProvider>();
            services.TryAddSingleton<IDryvScriptBlockGenerator, DryvScriptBlockGenerator>();
            services.TryAddSingleton<ITranslator, JavaScriptTranslator>();
            services.TryAddSingleton<DryvRulesFinder>();
            services.TryAddSingleton<DryvValidator>();
            services.TryAddSingleton<DryvServerRuleEvaluator>();
            services.TryAddSingleton<ITranslatorProvider, TranslatorProvider>();
            services.AddSingleton(Options.Create(options));

            var builder = new DryvMvcBuilder(services);
            builder
            .AddTranslator<ObjectTranslator>()
            .AddTranslator<DryvResultTranslator>()
            .AddTranslator<StringTranslator>()
            .AddTranslator<EnumerableTranslator>()
            .AddTranslator<RegexTranslator>()
            .AddTranslator<CustomCodeTranslator>();

            return builder;
        }
    }
}