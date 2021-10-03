using Dryv.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    public interface IDryvMvcBuilder : IDryvBuilder<IDryvMvcBuilder>
    {
        IMvcBuilder MvcBuilder { get; }
    }
}