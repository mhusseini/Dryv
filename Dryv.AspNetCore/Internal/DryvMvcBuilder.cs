using System;
using Dryv.Translation;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore.Internal
{
    internal class DryvMvcBuilder : IDryvMvcBuilder
    {
        public DryvMvcBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public IServiceCollection Services { get; }

        public IDryvMvcBuilder AddTranslator<T>()
        {
            var type = typeof(T);
            var used = false;

            if (typeof(IDryvMethodCallTranslator).IsAssignableFrom(type))
            {
                used = true;
                this.Services.AddSingleton(typeof(IDryvMethodCallTranslator), type);
            }

            if (typeof(IDryvCustomTranslator).IsAssignableFrom(type))
            {
                used = true;
                this.Services.AddSingleton(typeof(IDryvCustomTranslator), type);
            }

            if (!used)
            {
                throw new ArgumentException($"A custom translator must implement {nameof(IDryvMethodCallTranslator)} or {nameof(IDryvCustomTranslator)}.");
            }

            return this;
        }

        public IDryvMvcBuilder AddTranslator(object translator)
        {
            var used = false;

            if (translator is IDryvMethodCallTranslator methodCallTranslator)
            {
                used = true;
                this.Services.AddSingleton(methodCallTranslator);
            }

            if (translator is IDryvCustomTranslator customTranslator)
            {
                used = true;
                this.Services.AddSingleton(customTranslator);
            }

            if (!used)
            {
                throw new ArgumentException($"A custom translator must implement {nameof(IDryvMethodCallTranslator)} or {nameof(IDryvCustomTranslator)}.");
            }

            return this;
        }
    }
}