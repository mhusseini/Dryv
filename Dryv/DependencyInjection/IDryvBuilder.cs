using Microsoft.Extensions.DependencyInjection;

namespace Dryv.DependencyInjection
{
    public interface IDryvBuilder
    {
        IServiceCollection Services { get; }

        IDryvBuilder AddTranslator<T>();

        IDryvBuilder AddTranslator(object translator);
    }
}