namespace Dryv.Extensions
{
    public static class ResultExtensions
    {
        public static bool IsError(this DryvValidationResult result) => result?.Type == DryvResultType.Error;

        public static bool IsSuccess(this DryvValidationResult result) => result?.Type == DryvResultType.Success;

        public static bool IsWarning(this DryvValidationResult result) => result?.Type == DryvResultType.Warning;
    }
}