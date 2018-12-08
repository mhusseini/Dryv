using System.Collections.Generic;

namespace Dryv
{
    public interface IDryvScriptBlockGenerator
    {
        string GetScriptBody(IDictionary<string, string> validators);
    }
}