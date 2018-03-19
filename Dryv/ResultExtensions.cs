namespace Dryv
{
    internal static class ResultExtensions
    {
        public static bool IsError(this DryvResult result) => !result.IsSuccess();

        public static bool IsSuccess(this DryvResult result) => result == null || result == DryvResult.Success;
    }
}