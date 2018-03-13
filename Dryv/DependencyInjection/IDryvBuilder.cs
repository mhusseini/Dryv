using Microsoft.Extensions.DependencyInjection;

namespace Dryv.DependencyInjection
{
    public interface IDryvBuilder
    {
        IServiceCollection Services { get; }
    }
}