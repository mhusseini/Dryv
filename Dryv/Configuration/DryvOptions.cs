using System;

namespace Dryv.Configuration
{
    public class DryvOptions
    {
        public TranslationErrorBehavior TranslationErrorBehavior { get; set; }

        public Type ClientValidatorType { get; internal set; }

        public Type ClientBodyGeneratorType { get; internal set; }

        public bool BreakOnFirstValidationError { get; set; } = true;
    }
}