using Dryv.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore.Internal
{
    internal class DryvMvcBuilder : IDryvMvcBuilder
    {
        public DryvMvcBuilder(DryvOptions options, IMvcBuilder mvcBuilder)
        {
            this.Options = options;
            this.MvcBuilder = mvcBuilder;
        }

        public DryvOptions Options { get; }
        public IMvcBuilder MvcBuilder { get; }
    }
}