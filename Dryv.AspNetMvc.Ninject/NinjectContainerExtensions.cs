using Dryv.Configuration;
using Ninject;

namespace Dryv.AspNetMvc
{
    public static class NinjectContainerExtensions
    {
        public static IDryvBuilder RegisterDryv(this IKernel kernel)
            => DryvMvc.Configure(new DependencyContainer(kernel));
    }
}