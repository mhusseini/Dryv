using System.Threading;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Dryv.DynamicControllers
{
    internal class DryvDynamicActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            this.TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(this.TokenSource.Token);
        }
    }
}