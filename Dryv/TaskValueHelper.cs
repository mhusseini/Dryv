using System.Threading.Tasks;

namespace Dryv
{
    internal static class TaskValueHelper
    {
        public static async Task<object> GetPossiblyAsyncValue(object value)
        {
            switch (value)
            {
                case Task task:
                    var result = await task.OfObject();
                    return result;
                default:
                    return value;
            }
        }
    }
}