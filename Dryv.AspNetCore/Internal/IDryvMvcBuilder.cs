using Dryv.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore.Internal
{
    public interface IDryvMvcBuilder : IDryvBuilder<IDryvMvcBuilder>
    {
        public IServiceCollection Services { get; }
    }
}