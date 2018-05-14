using Dryv.Configuration;
using SimpleInjector;

namespace Dryv.AspNetMvc
{
    public static class SimpleInjectorContainerExtensions
    {
        public static IDryvBuilder RegisterDryv(this Container container)
            => DryvMvc.Configure(new DependencyContainer(container));
    }
}