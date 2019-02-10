using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Dryv.Extensions;

namespace Dryv.Validation
{
    [DebuggerDisplay("{Path}.{Property.Name}")]
    public struct DryvResult
    {
        public DryvResult(object model, PropertyInfo property, string path, IEnumerable<DryvResultMessage> messages)
        {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.Property = property ?? throw new ArgumentNullException(nameof(property));
            this.Message = messages?.Where(m => !m.IsSuccess()).ToList() ?? throw new ArgumentNullException(nameof(messages)); ;
            this.Path = path ?? string.Empty;
        }

        public IReadOnlyCollection<DryvResultMessage> Message { get; }
        public object Model { get; }
        public string Path { get; }
        public PropertyInfo Property { get; }
    }
}