using System;
using System.Collections.Generic;
using System.IO;

namespace Dryv.Validation
{
    public interface IDryvClientValidationSetWriter
    {
        void WriteBegin(TextWriter writer);

        void WriteEnd(TextWriter writer);

        void WriteValidationSet(TextWriter writer, string validationSetName, IDictionary<string, Action<TextWriter>> validators, IDictionary<string, Action<TextWriter>> disablers, IDictionary<string, object> parameters);
    }
}