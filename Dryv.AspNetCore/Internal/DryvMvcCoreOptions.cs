using System;
using Dryv.Configuration;

namespace Dryv.AspNetCore.Internal
{
    internal class DryvMvcCoreOptions : DryvOptions
    {
        public DryvMvcCoreOptions()
        {
        }

        public DryvMvcCoreOptions(Action<Type> typeAdded)
        {
            this.Translators = new RegisteringDryvServiceCollection(typeAdded);
            this.Annotators = new RegisteringDryvServiceCollection(typeAdded);
        }

        public override DryvServiceCollection Annotators { get; }
        public override DryvServiceCollection Translators { get; }
    }
}