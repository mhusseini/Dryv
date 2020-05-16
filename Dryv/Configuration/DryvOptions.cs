using System;
using Dryv.Validation;

namespace Dryv.Configuration
{
    public class DryvOptions
    {
        public TranslationErrorBehavior TranslationErrorBehavior { get; set; }

        public Type ClientFunctionWriterType { get; internal set; } = typeof(DryvClientValidationFunctionWriter);

        public bool BreakOnFirstValidationError { get; set; } = true;
    }
}