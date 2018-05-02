using System;
using StructureMap;

namespace Dryv.AspNetMvc
{
    internal class DependencyContainer : IDependencyContainer
    {
        private readonly ConfigurationExpression configuration;

        public DependencyContainer(ConfigurationExpression configuration) => this.configuration = configuration;

        public void AddInstance(Type iface, object implementation) => this.configuration.ForSingletonOf(iface).Use(implementation).Named(Guid.NewGuid().ToString());

        public void AddSingleton(Type iface, Type implementation) => this.configuration.ForSingletonOf(iface).Use(implementation).Named(Guid.NewGuid().ToString());

        public void RegisterInstance(Type iface, object implementation) => this.configuration.ForSingletonOf(iface).Use(implementation);

        public void RegisterSingleton(Type iface, Type implementation) => this.configuration.ForSingletonOf(iface).Use(implementation);
    }
}