using System;
using SimpleInjector;

namespace Dryv.AspNetMvc
{
    internal class DependencyContainer : IDependencyContainer
    {
        private readonly Container container;

        public DependencyContainer(Container container) => this.container = container;

        public void AddInstance(Type iface, object implementation) =>
            this.container.Collections.AppendTo(
            iface,
            Lifestyle.Singleton.CreateRegistration(iface, () => implementation, this.container));

        public void AddSingleton(Type iface, Type implementation) =>
            this.container.Collections.AppendTo(iface, implementation);

        public void RegisterInstance(Type iface, object implementation) =>
            this.container.RegisterInstance(iface, implementation);

        public void RegisterSingleton(Type iface, Type implementation) =>
            this.container.RegisterSingleton(iface, implementation);
    }
}