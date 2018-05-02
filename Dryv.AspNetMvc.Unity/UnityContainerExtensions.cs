using Dryv.Configuration;
using Unity;

namespace Dryv.AspNetMvc
{
    public static class UnityContainerExtensions
    {
        public static IDryvBuilder RegisterDryv(this IUnityContainer container)
            => DryvMvc.Configure(new DependencyContainer(container));
    }
}