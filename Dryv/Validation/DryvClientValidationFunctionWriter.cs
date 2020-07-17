using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dryv.Validation
{
    public class DryvClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        public Action<TextWriter> GetValidationFunction(IEnumerable<string> translatedRules) =>
            writer =>
                writer.Write($@"function(m) {{ return {string.Join("||", translatedRules.Select(f => $"({f}).call(this, m)"))}; }}");
    }
}