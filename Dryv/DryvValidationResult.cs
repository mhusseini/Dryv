using System.Diagnostics;

namespace Dryv
{
    [DebuggerDisplay("{Type}: {Text}")]
    public sealed class DryvValidationResult
    {
        public static readonly DryvValidationResult Success = new DryvValidationResult(DryvResultType.Success);

        public DryvValidationResult(string text, DryvResultType type, string groupName) : this(text, type) => this.GroupName = groupName;

        public DryvValidationResult(string text, DryvResultType type) : this(type) => this.Text = text;

        private DryvValidationResult(DryvResultType type) => this.Type = type;

        public string GroupName { get; internal set; }

        public string Text { get; }

        public DryvResultType Type { get; }

        public static DryvValidationResult Error(string text) => new DryvValidationResult(text, DryvResultType.Error);

        public static implicit operator DryvValidationResult(string text) => string.IsNullOrWhiteSpace(text) ? Success : Error(text);

        public static DryvValidationResult Warning(string text) => new DryvValidationResult(text, DryvResultType.Warning);
    }
}