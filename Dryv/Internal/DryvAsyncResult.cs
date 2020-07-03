using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Validation;

namespace Dryv.Internal
{
    internal struct DryvAsyncResult
    {
        public DryvAsyncResult(object model, PropertyInfo property, string path, Task<IReadOnlyCollection<DryvValidationResult>> task)
        {
            this.Task = task.ContinueWith(t => new DryvResult(model, property, path, t.Result));
        }

        public Task<DryvResult> Task { get; }
    }
}