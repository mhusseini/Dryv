using System;

namespace Dryv.Configuration
{
    public class DryvOptions
    {
        public TranslationErrorBehavior TranslationErrorBehavior { get; set; }

        internal Type ClientModelValidatorType { get; set; }
    }
}