using Dryv.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore.Internal
{
    public interface IDryvMvcBuilder : IDryvBuilder
    {
        public IServiceCollection Services { get; }
    }
}