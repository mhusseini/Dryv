using System;
using System.Linq;
using System.Reflection;
using Dryv.RuleDetection;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.PreLoading
{
    internal class DryvPreloader : IDisposable
    {
        private readonly DryvRuleFinder ruleFinder;
        private readonly IOptions<DryvPreloaderOptions> options;
        private bool hasStarted;

        public DryvPreloader(DryvRuleFinder ruleFinder, IOptions<DryvPreloaderOptions> options)
        {
            this.ruleFinder = ruleFinder;
            this.options = options;
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyLoad -= this.CurrentDomainOnAssemblyLoad;
        }

        public void Preload()
        {
            if (this.hasStarted)
            {
                return;
            }

            this.hasStarted = true;

            AppDomain.CurrentDomain.AssemblyLoad += this.CurrentDomainOnAssemblyLoad;
            this.Preload(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Preload(params Assembly[] assemblies)
        {
            if (!this.options.Value.IsEnabled)
            {
                return;
            }

            foreach (var type in from t in assemblies.SelectMany(a => a.GetTypes())
                                 where IsValidatable(t)
                                 select t)
            {
                this.ruleFinder.FindValidationRulesInTree(type, RuleType.Default);
                this.ruleFinder.FindValidationRulesInTree(type, RuleType.Disabling);
            }
        }

        private static bool IsValidatable(Type type)
        {
            if (type.GetCustomAttribute<DryvValidationAttribute>() != null)
            {
                return true;
            }

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
            return props.Any(p => p.GetCustomAttribute<DryvValidationAttribute>() != null);
        }

        private void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            this.Preload(args.LoadedAssembly);
        }
    }
}