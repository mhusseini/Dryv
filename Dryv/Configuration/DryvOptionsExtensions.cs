namespace Dryv.Configuration
{
    public static class DryvOptionsExtensions
    {
        public static void UseClientModelValidator<T>(this DryvOptions options)
            where T : IDryvClientModelValidator
        {
            options.ClientModelValidatorType = typeof(T);
        }
    }
}