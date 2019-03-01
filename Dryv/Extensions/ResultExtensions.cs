namespace Dryv.Extensions
{
    public static class ResultExtensions
    {
        public static bool IsError(this DryvResultMessage result) => result?.Type == DryvResultType.Error;

        public static bool IsSuccess(this DryvResultMessage result) => result?.Type == DryvResultType.Success;

        public static bool IsWarning(this DryvResultMessage result) => result?.Type == DryvResultType.Warning;
    }
}