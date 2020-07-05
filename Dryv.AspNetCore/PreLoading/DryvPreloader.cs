using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.PreLoading
{
    internal class DryvPreloader : IDisposable
    {
        private readonly IOptions<DryvPreloaderOptions> options;
        private readonly DryvClientValidationLoader preLoader;
        private bool hasStarted;

        public DryvPreloader(IOptions<DryvPreloaderOptions> options, DryvClientValidationLoader preLoader)
        {
            this.options = options;
            this.preLoader = preLoader;
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
                                 let props = t.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                                 where props.Any(p => p.GetCustomAttribute<DryvValidationAttribute>() != null)
                                 select t)
            {
                this.preLoader.GetDryvClientValidation(type);
            }
        }

        private void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            this.Preload(args.LoadedAssembly);
        }
    }
}