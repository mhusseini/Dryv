using System;
using System.Linq;
using System.Reflection;
using Dryv.RuleDetection;
using Dryv.Rules;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.PreLoading
{
    internal class DryvPreloader : IDisposable
    {
        private readonly IOptions<DryvPreloadingOptions> options;
        private readonly DryvRuleFinder ruleFinder;
        private bool hasStarted;

        public DryvPreloader(DryvRuleFinder ruleFinder, IOptions<DryvPreloadingOptions> options)
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

        private void Preload(params Assembly[] assemblies)
        {
            if (!this.options.Value.IsEnabled)
            {
                return;
            }

            foreach (var (type, _) in DryvSets.GetDryvSets())
            {
                this.ruleFinder.FindValidationRulesInTree(type, RuleType.Validation);
                this.ruleFinder.FindValidationRulesInTree(type, RuleType.Disabling);
            }
        }

        private void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            this.Preload(args.LoadedAssembly);
        }
    }
}