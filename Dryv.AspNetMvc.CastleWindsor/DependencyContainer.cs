using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Dryv.AspNetMvc
{
    internal class DependencyContainer : IDependencyContainer
    {
        private readonly IWindsorContainer container;

        public DependencyContainer(IWindsorContainer container) => this.container = container;

        public void AddInstance(Type iface, object implementation) => this.container.Register(Component.For(iface).Instance(implementation).LifestyleSingleton().Named(GetRandomName()));

        public void AddSingleton(Type iface, Type implementation) => this.container.Register(Component.For(iface).ImplementedBy(implementation).LifestyleSingleton().Named(GetRandomName()));

        public void RegisterInstance(Type iface, object implementation) => this.container.Register(Component.For(iface).Instance(implementation).LifestyleSingleton());

        public void RegisterSingleton(Type iface, Type implementation) => this.container.Register(Component.For(iface).ImplementedBy(implementation).LifestyleSingleton());

        private static string GetRandomName() => Guid.NewGuid().ToString();
    }
}