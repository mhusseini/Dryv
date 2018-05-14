using System;
using Ninject;

namespace Dryv.AspNetMvc
{
    internal class DependencyContainer : IDependencyContainer
    {
        private readonly IKernel kernel;

        public DependencyContainer(IKernel kernel) => this.kernel = kernel;

        public void AddInstance(Type iface, object implementation) => this.kernel.Bind(iface).ToConstant(implementation).InSingletonScope().Named(Guid.NewGuid().ToString());

        public void AddSingleton(Type iface, Type implementation) => this.kernel.Bind(iface).To(implementation).InSingletonScope().Named(Guid.NewGuid().ToString());

        public void RegisterInstance(Type iface, object implementation) => this.kernel.Bind(iface).ToConstant(implementation).InSingletonScope();

        public void RegisterSingleton(Type iface, Type implementation) => this.kernel.Bind(iface).To(implementation).InSingletonScope();
    }
}