using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DryvDemo.TagHelpers
{
    public interface IHostingEnvironmentAwareViewComponent
    {
        IHostingEnvironment HostingEnvironment { get; }

        ViewContext ViewContext { get; }
    }
}