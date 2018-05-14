using System;
using System.Collections.Generic;

namespace Dryv.AspNetMvc
{
    public interface IDependencyContainer
    {
        void AddInstance(Type iface, object implementation);

        void AddSingleton(Type iface, Type implementation);

        void RegisterInstance(Type iface, object implementation);

        void RegisterSingleton(Type iface, Type implementation);
    }
}