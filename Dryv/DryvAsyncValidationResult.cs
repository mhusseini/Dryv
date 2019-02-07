using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Dryv
{
    internal struct DryvAsyncValidationResult
    {
        public DryvAsyncValidationResult(object model, PropertyInfo property, string path, Task<IReadOnlyCollection<DryvResultMessage>> task)
        {
            this.Model = model;
            this.Property = property;
            this.Path = path;
            this.Task = task;
        }

        public object Model { get; }
        public string Path { get; }
        public PropertyInfo Property { get; }
        public Task<IReadOnlyCollection<DryvResultMessage>> Task { get; }
    }
}