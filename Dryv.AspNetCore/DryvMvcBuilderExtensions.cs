using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.AspNetCore.Internal;
using Dryv.Configuration;
using Dryv.Rework;
using Dryv.Rework.Compilation;
using Dryv.Rework.RuleDetection;
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

            if (options.JsonConversion == null)
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                options.JsonConversion = v => JsonSerializer.Serialize(v, jsonOptions);
            }

            if (!options.DisableAutomaticValidation)
            {
                mvcBuilder.AddMvcOptions(opts => opts.Filters.Add<DryvValidationFilterAttribute>());
            }

            return RegsterServices(mvcBuilder.Services, options);
        }

        private static IDryvMvcBuilder RegsterServices(this IServiceCollection services, DryvOptions options)
        {
            services.TryAddSingleton<DryvEndpointRouteBuilderProvider>();
            services.TryAddSingleton(typeof(IDryvClientValidationFunctionWriter), options.ClientFunctionWriterType ?? DryvOptions.DefaultClientFunctionWriterType);
            services.TryAddSingleton(typeof(IDryvClientValidationSetWriter), options.ClientValidationSetWriterType ?? DryvOptions.DefaultClientValidationSetWriterType);
            services.TryAddSingleton<ITranslator, JavaScriptTranslator>();
            services.TryAddSingleton<ModelTreeBuilder>();
            services.TryAddSingleton<DryvValidator>();
            services.TryAddSingleton<DryvCompiler>();
            services.TryAddSingleton<DryvTranslator>();
            services.TryAddSingleton<DryvRuleFinder>();
            services.TryAddSingleton<DryvClientWriter>();
            services.TryAddSingleton<ITranslatorProvider, TranslatorProvider>();
            services.AddSingleton(Options.Create(options));
            services.AddSingleton(options);

            return new DryvMvcBuilder(services)
                .AddTranslator<ObjectTranslator>()
                .AddTranslator<DryvValidationResultTranslator>()
                .AddTranslator<StringTranslator>()
                .AddTranslator<EnumerableTranslator>()
                .AddTranslator<RegexTranslator>()
                .AddTranslator<CustomCodeTranslator>();
        }
    }
}