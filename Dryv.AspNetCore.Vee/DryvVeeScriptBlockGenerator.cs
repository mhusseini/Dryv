using System.Collections.Generic;
using System.Linq;

namespace Dryv.AspNetCore
{
    public class DryvVeeScriptBlockGenerator : DryvScriptBlockGenerator
    {
        public override string GetScriptBody(IDictionary<string, string> validators)
        {
            return validators.Any()
               ? @"<script>
                    (function(w){
                        var a = w.dryv = (w.dryv || {});
                        var v = a.vee = (a.vee || {});
                        " + string.Concat(validators.Select(i =>
                        $"v.{i.Key} = {{v:{i.Value}}};")) + @"
                    })(window);
                </script>"
               : null;
        }

    }
}