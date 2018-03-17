namespace Dryv
{
    public class DryvResult
    {
        public static readonly DryvResult Success = new DryvResult();

        public DryvResult()
        {
        }

        public DryvResult(string message) => this.Message = message;

        public string Message { get; }

        public static DryvResult Fail(string message) => new DryvResult(message);

        public static implicit operator DryvResult(string message) => new DryvResult(message);
    }
}