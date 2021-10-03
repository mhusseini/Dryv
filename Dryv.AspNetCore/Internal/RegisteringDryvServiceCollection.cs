using System;
using Dryv.Configuration;

namespace Dryv.AspNetCore.Internal
{
    internal class RegisteringDryvServiceCollection : DryvServiceCollection
    {
        private readonly Action<Type> typeAdded;

        public RegisteringDryvServiceCollection(Action<Type> typeAdded)
        {
            this.typeAdded = typeAdded;
        }

        protected override void OnAdd(Type type)
        {
            this.typeAdded(type);
        }
    }
}