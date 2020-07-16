using System;
using System.Collections.Generic;
using System.IO;

namespace Dryv.Validation
{
    public interface IDryvClientValidationSetWriter
    {
        void WriteBegin(TextWriter writer, Func<Type, object> serviceProvider);

        void WriteEnd(TextWriter writer, Func<Type, object> serviceProvider);

        void WriteValidationSet(TextWriter writer, string validationSetName, IDictionary<string, Action<Func<Type, object>, TextWriter>> validators, IDictionary<string, Action<Func<Type, object>, TextWriter>> disablers, Func<Type, object> serviceProvider);
    }
}