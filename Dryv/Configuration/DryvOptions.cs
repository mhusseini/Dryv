using System;

namespace Dryv.Configuration
{
    public class DryvOptions
    {
        public TranslationErrorBehavior TranslationErrorBehavior { get; set; }

        public Type ClientValidatorType { get; private set; }

        public void UseClientValidator<T>() where T : IDryvClientValidationProvider
        {
            this.ClientValidatorType = typeof(T);
        }

        public Type ClientBodyGeneratorType { get; private set; }

        public void UseClientBodyGenerator<T>() where T : IDryvScriptBlockGenerator
        {
            this.ClientBodyGeneratorType = typeof(T);
        }
    }
}