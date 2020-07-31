using System.Collections.Generic;
using System.Diagnostics;

namespace Dryv
{
    [DebuggerDisplay("{Type}: {Text}")]
    public sealed class DryvValidationResult
    {
        public static readonly DryvValidationResult Success = new DryvValidationResult(DryvResultType.Success, null);

        public DryvValidationResult(string text, DryvResultType type, object data, string group) : this(text, type, data) => this.Group = group;

        public DryvValidationResult(string text, DryvResultType type, object data) : this(type, data) => this.Text = text;

        public DryvValidationResult(string text, DryvResultType type) : this(type, null) => this.Text = text;

        private DryvValidationResult(DryvResultType type, object data)
        {
            this.Type = type;
            this.Data = data;
        }

        public object Data { get; }
        public string Group { get; internal set; }

        public string Text { get; }

        public DryvResultType Type { get; }

        public static DryvValidationResult Error(string text, object data) => new DryvValidationResult(text, DryvResultType.Error, data);

        public static DryvValidationResult Error(string text) => Error(text, null);

        public static implicit operator DryvValidationResult(string text) => string.IsNullOrWhiteSpace(text) ? Success : Error(text);

        public static DryvValidationResult Warning(string text, object data) => new DryvValidationResult(text, DryvResultType.Warning, data);

        public static DryvValidationResult Warning(string text) => Warning(text, null);
    }
}