using System;
using Dryv.Configuration;
using Dryv.Translation;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore.Internal
{
    internal class DryvBuilder : IDryvBuilder
    {
        public DryvBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public IServiceCollection Services { get; }

        public IDryvBuilder AddTranslator<T>()
        {
            var type = typeof(T);
            var used = false;

            if (typeof(IMethodCallTranslator).IsAssignableFrom(type))
            {
                used = true;
                this.Services.AddSingleton(typeof(IMethodCallTranslator), type);
            }

            if (typeof(ICustomTranslator).IsAssignableFrom(type))
            {
                used = true;
                this.Services.AddSingleton(typeof(ICustomTranslator), type);
            }

            if (!used)
            {
                throw new ArgumentException($"A custom translator must implement {nameof(IMethodCallTranslator)} or {nameof(ICustomTranslator)}.");
            }

            return this;
        }

        public IDryvBuilder AddTranslator(object translator)
        {
            var used = false;

            if (translator is IMethodCallTranslator methodCallTranslator)
            {
                used = true;
                this.Services.AddSingleton(methodCallTranslator);
            }

            if (translator is ICustomTranslator customTranslator)
            {
                used = true;
                this.Services.AddSingleton(customTranslator);
            }

            if (!used)
            {
                throw new ArgumentException($"A custom translator must implement {nameof(IMethodCallTranslator)} or {nameof(ICustomTranslator)}.");
            }

            return this;
        }
    }
}