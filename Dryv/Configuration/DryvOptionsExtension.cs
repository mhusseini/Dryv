using Dryv.Translation;
using Dryv.Validation;

namespace Dryv.Configuration
{
    public static class DryvOptionsExtension
    {
        public static void UseClientFunctionWriter<T>(this DryvOptions options) where T : IDryvClientValidationFunctionWriter
        {
            options.ClientFunctionWriterType = typeof(T);
        }
    }
}