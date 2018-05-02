using System;
using System.Collections.Generic;
using SimpleInjector;

namespace Dryv.AspNetMvc
{
    internal class DependencyContainer : IDependencyContainer
    {
        private readonly Container container;
        private readonly HashSet<Type> registeredCollections = new HashSet<Type>();

        public DependencyContainer(Container container) => this.container = container;

        public void AddInstance(Type iface, object implementation)
        {
            if (!this.registeredCollections.Contains(iface))
            {
                this.container.RegisterCollection(iface, implementation);
                this.registeredCollections.Add(iface);
            }
            else
            {
                this.container.Collections.AppendTo(
                    iface,
                    Lifestyle.Singleton.CreateRegistration(() => implementation, this.container));
            }
        }

        public void AddSingleton(Type iface, Type implementation)
        {
            if (!this.registeredCollections.Contains(iface))
            {
                this.container.RegisterCollection(iface, implementation);
            }
            else
            {
                this.container.Collections.AppendTo(iface, implementation);
            }
        }

        public void RegisterInstance(Type iface, object implementation) => this.container.RegisterInstance(iface, implementation);

        public void RegisterSingleton(Type iface, Type implementation) => this.container.RegisterSingleton(iface, implementation);
    }
}