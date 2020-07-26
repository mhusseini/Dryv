using System;
using System.Collections.Generic;
using System.IO;

namespace Dryv.Validation
{
    public interface IDryvClientValidationFunctionWriter
    {
        Action<TextWriter> GetValidationFunction(IEnumerable<TranslatedRule> translatedRules);
    }
}