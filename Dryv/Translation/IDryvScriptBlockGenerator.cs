using System.Collections.Generic;
using Dryv.Validation;

namespace Dryv.Translation
{
    public interface IDryvScriptBlockGenerator
    {
        string GetScriptBody(IDictionary<string, DryvClientPropertyValidation> validators);
    }
}