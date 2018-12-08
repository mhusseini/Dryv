using System.Linq;
using System.Collections.Generic;

namespace Dryv
{
    public class DryvScriptBlockGenerator : IDryvScriptBlockGenerator
    {
        public virtual string GetScriptBody(IDictionary<string, string> validators)
        {
            return validators.Any()
               ? @"<script>
                    (function(w){
                        var a = w.dryv = w.dryv || {};
                        " + string.Concat(validators.Select(i => $"a.{i.Key} = {i.Value};")) + @";
                    })(window);
                </script>"
               : null;
        }

    }
}