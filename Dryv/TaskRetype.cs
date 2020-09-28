using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Reflection;

namespace Dryv
{
    internal static class TaskRetype
    {
        private static readonly ConcurrentDictionary<Type, Func<Task, Task<object>>> Wrappers = new ConcurrentDictionary<Type, Func<Task, Task<object>>>();
        private static readonly MethodInfo FactoryMethod = typeof(TaskRetype).GetMethod(nameof(GenericWapper));

        public static async Task<object> OfObject(this Task task)
        {
            var taskType = task.GetType();
            if (taskType.GenericTypeArguments.Length == 0)
            {
                return NonGenericWrapper(task);
            }

            var genericArg = taskType.GenericTypeArguments[0];
            var wrapper = Wrappers.GetOrAdd(genericArg, GenerateWrapperFactory);

            return await wrapper(task);
        }

        private static Func<Task, Task<object>> GenerateWrapperFactory(Type genericArg)
        {
            var parameter = Expression.Parameter(typeof(Task));
            var lambda = Expression.Lambda<Func<Task, Task<object>>>(
                Expression.Call(
                    FactoryMethod.MakeGenericMethod(genericArg),
                    parameter),
                parameter);

            return lambda.Compile();
        }

        private static async Task<object> GenericWapper<T>(Task inner)
        {
            var result = await (Task<T>) inner;
            return result;
        }

        private static async Task<object> NonGenericWrapper(Task inner)
        {
            await inner;
            return Task.FromResult(default(object));
        }
    }
}