using System.Diagnostics;
using System.Reflection;

namespace Dryv
{
    [DebuggerDisplay("{" + nameof(Path) + "}: {" + nameof(ValidationResult) + "}")]
    public struct DryvValidationResult
    {
        public DryvValidationResult(object model, PropertyInfo property, string path, DryvResult validationResult)
        {
            this.Model = model;
            this.Property = property;
            this.Path = path;
            this.ValidationResult = validationResult;
        }

        public object Model { get; }
        public string Path { get; }
        public PropertyInfo Property { get; }
        public DryvResult ValidationResult { get; }
    }
}