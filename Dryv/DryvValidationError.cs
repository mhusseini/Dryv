using System.Diagnostics;
using System.Reflection;

namespace Dryv
{
    [DebuggerDisplay("{" + nameof(Path) + "}: {" + nameof(ErrorMessage) + "}")]
    public struct DryvValidationError
    {
        public DryvValidationError(object model, PropertyInfo property, string path, string errorMessage)
        {
            this.Model = model;
            this.Property = property;
            this.Path = path;
            this.ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
        public object Model { get; }
        public string Path { get; }
        public PropertyInfo Property { get; }

        public override string ToString() => $"{this.Path}: {this.ErrorMessage}";
    }
}