using Dryv.Translation;
using Dryv.Validation;

namespace Dryv.Configuration
{
    public static class DryvOptionsExtension
    {
        public static void UseClientValidator<T>(this DryvOptions options) where T : IDryvClientValidationProvider
        {
            options.ClientValidatorType = typeof(T);
        }

        public static void UseClientBodyGenerator<T>(this DryvOptions options) where T : IDryvScriptBlockGenerator
        {
            options.ClientBodyGeneratorType = typeof(T);
        }
    }
}