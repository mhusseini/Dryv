using Microsoft.Extensions.DependencyInjection;

namespace Dryv.DependencyInjection
{
    public class DryvBuilder : IDryvBuilder
    {
        public IServiceCollection Services { get; }

        public DryvBuilder(IServiceCollection services)
        {
            this.Services = services;
        }
    }
}