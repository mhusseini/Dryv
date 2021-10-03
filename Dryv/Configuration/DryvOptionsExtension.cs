using Dryv.Translation;
using Dryv.Validation;

namespace Dryv.Configuration
{
    public static class DryvOptionsExtension
    {
        public static void UseClientValidationFunctionWriter<T>(this DryvOptions options) where T : IDryvClientValidationFunctionWriter
        {
            options.ClientFunctionWriterType = typeof(T);
        }

        public static void UseClientValidationSetWriter<T>(this DryvOptions options) where T : IDryvClientValidationSetWriter
        {
            options.ClientValidationSetWriterType = typeof(T);
        }
    }
}