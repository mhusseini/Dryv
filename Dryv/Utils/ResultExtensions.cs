namespace Dryv.Utils
{
    public static class ResultExtensions
    {
        public static bool IsError(this DryvResultMessage result) => !result.IsSuccess();

        public static bool IsSuccess(this DryvResultMessage result) => result == null || result == DryvResultMessage.Success;
    }
}