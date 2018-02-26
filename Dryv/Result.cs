namespace Dryv
{
    public class Result
    {
        public static readonly Result Success = new Result();

        public Result()
        {
        }

        public Result(string message)
        {
            this.Message = message;
        }

        public string Message { get; }

        public static Result Error(string message) => new Result(message);

        public static implicit operator Result(string message) => new Result(message);
    }
}