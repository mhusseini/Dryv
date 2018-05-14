using System;
using Unity;

namespace Dryv.AspNetMvc
{
    internal class DependencyContainer : IDependencyContainer
    {
        private readonly IUnityContainer container;

        public DependencyContainer(IUnityContainer container) => this.container = container;

        public void AddInstance(Type iface, object implementation) => this.container.RegisterInstance(iface, GetRandomName(), implementation);

        public void AddSingleton(Type iface, Type implementation) => this.container.RegisterSingleton(iface, implementation, GetRandomName());

        public void RegisterInstance(Type iface, object implementation) => this.container.RegisterInstance(iface, implementation);

        public void RegisterSingleton(Type iface, Type implementation) => this.container.RegisterSingleton(iface, implementation);

        private static string GetRandomName() => Guid.NewGuid().ToString();
    }
}