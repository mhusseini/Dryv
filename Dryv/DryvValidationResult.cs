using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Dryv.Utils;

namespace Dryv
{
    [DebuggerDisplay("{Path}.{Property.Name}")]
    public struct DryvValidationResult
    {
        public DryvValidationResult(object model, PropertyInfo property, string path, IEnumerable<DryvResultMessage> messages)
        {
            this.Model = model;
            this.Property = property;
            this.Path = path;
            this.Message = messages.Where(m => !m.IsSuccess()).ToList();
        }

        public IReadOnlyCollection<DryvResultMessage> Message { get; }
        public object Model { get; }
        public string Path { get; }
        public PropertyInfo Property { get; }
    }
}