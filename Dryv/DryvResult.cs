namespace Dryv
{
    public sealed class DryvResult
    {
        public static readonly DryvResult Success = new DryvResult(DryvResultType.Success);

        private DryvResult(DryvResultType type) => this.Type = type;

        private DryvResult(string message, DryvResultType type) : this(type) => this.Message = message;

        public string Message { get; }

        public DryvResultType Type { get; }

        public static DryvResult Error(string message) => new DryvResult(message, DryvResultType.Error);

        public static implicit operator DryvResult(string message) => string.IsNullOrWhiteSpace(message) ? Success : Error(message);

        public static DryvResult Warning(string message) => new DryvResult(message, DryvResultType.Warning);
    }
}