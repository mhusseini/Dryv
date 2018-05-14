using Dryv.Configuration;
using StructureMap;

namespace Dryv.AspNetMvc
{
    public static class StructureMapContainerExtensions
    {
        public static IDryvBuilder RegisterDryv(this ConfigurationExpression configuration)
            => DryvMvc.Configure(new DependencyContainer(configuration));
    }
}