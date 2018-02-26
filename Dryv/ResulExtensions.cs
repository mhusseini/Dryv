namespace Dryv
{
    public static class ResulExtensions
    {
        public static bool IsError(this Result result) => !result.IsSuccess();

        public static bool IsSuccess(this Result result) => result == null || result == Result.Success;
    }
}