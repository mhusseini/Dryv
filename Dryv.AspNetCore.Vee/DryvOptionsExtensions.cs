using Dryv.Configuration;

namespace Dryv.AspNetCore
{
    public static class DryvOptionsExtensions
    {
        public static void UseVeeValidate(this DryvOptions options)
        {
            options.UseClientValidator<DryvVeeValidateModelValidator>();
            options.UseClientBodyGenerator<DryvVeeScriptBlockGenerator>();
        }
    }
}