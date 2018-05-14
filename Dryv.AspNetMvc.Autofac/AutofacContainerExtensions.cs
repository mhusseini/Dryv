using Autofac;
using Dryv.Configuration;

namespace Dryv.AspNetMvc
{
    public static class AutofacContainerExtensions
    {
        public static IDryvBuilder RegisterDryv(this ContainerBuilder builder)
            => DryvMvc.Configure(new DependencyContainer(builder));
    }
}