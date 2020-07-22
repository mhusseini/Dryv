using System;

namespace Dryv
{
    public class DryvDependencyInjectionException : DryvException
    {
        public DryvDependencyInjectionException(Type type, Exception innerException) : base($"The service provider could not return an implementation for type {type.FullName}. Did you forget to register it? ", innerException)
        {
        }

        public DryvDependencyInjectionException(Type type) : this(type, null)
        {
        }
    }
}