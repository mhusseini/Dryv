using System;

namespace Dryv.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this Func<Type, object> serviceProvider)
            where T : class
        {
            return serviceProvider(typeof(T)) as T;
        }
    }
}