namespace Dryv
{
    public class DryvResult
    {
        public static readonly DryvResult Success = new DryvResult(DryResulType.Success);

        private DryvResult(DryResulType type) => this.Type = type;

        private DryvResult(string message, DryResulType type) : this(type) => this.Message = message;

        public string Message { get; }

        public DryResulType Type { get; }

        public static DryvResult Error(string message) => new DryvResult(message, DryResulType.Error);

        public static implicit operator DryvResult(string message) => Error(message);

        public static DryvResult Warning(string message) => new DryvResult(message, DryResulType.Warning);
    }
}