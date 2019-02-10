using System.Diagnostics;

namespace Dryv
{
    [DebuggerDisplay("{Type} - {Text}")]
    public sealed class DryvResultMessage
    {
        public static readonly DryvResultMessage Success = new DryvResultMessage(DryvResultType.Success);

        private DryvResultMessage(DryvResultType type) => this.Type = type;

        private DryvResultMessage(string text, DryvResultType type) : this(type) => this.Text = text;

        public string Text { get; }

        public DryvResultType Type { get; }

        public static DryvResultMessage Error(string text) => new DryvResultMessage(text, DryvResultType.Error);

        public static implicit operator DryvResultMessage(string text) => string.IsNullOrWhiteSpace(text) ? Success : Error(text);

        public static DryvResultMessage Warning(string text) => new DryvResultMessage(text, DryvResultType.Warning);
    }
}