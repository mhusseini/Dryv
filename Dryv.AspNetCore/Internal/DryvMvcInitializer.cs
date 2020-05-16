using System;

namespace Dryv.AspNetCore.Internal
{
    internal class DryvMvcInitializer
    {
        private readonly Action<IServiceProvider> action;

        public DryvMvcInitializer(Action<IServiceProvider> action) => this.action = action ?? throw new ArgumentNullException();

        public void Run(IServiceProvider serviceProvider) => this.action(serviceProvider);
    }
}