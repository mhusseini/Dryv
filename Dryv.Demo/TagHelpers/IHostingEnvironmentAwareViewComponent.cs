using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dryv.Demo.TagHelpers
{
    public interface IHostingEnvironmentAwareViewComponent
    {
        IHostingEnvironment HostingEnvironment { get; }

        ViewContext ViewContext { get; }
    }
}