using Castle.Windsor;
using Dryv.Configuration;

namespace Dryv.AspNetMvc
{
    public static class CastleWindsorContainerExtensions
    {
        public static IDryvBuilder RegisterDryv(this IWindsorContainer container)
            => DryvMvc.Configure(new DependencyContainer(container));
    }
}