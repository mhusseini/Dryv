using System.Linq;
using System.Collections.Generic;

namespace Dryv
{
    public class DryvVeeScriptBlockGenerator : DryvScriptBlockGenerator
    {
        public override string GetScriptBody(IDictionary<string, string> validators)
        {
            return validators.Any()
               ? @"<script>
                    (function(w){
                        var a = w.dryv = w.dryv || {};
                        var v = a.vee = a.vee || {};
                        " + string.Concat(validators.Select(i =>
                        $"v.{i.Key} = {{m: null, v:{i.Value}}};")) + @"
                    })(window);
                </script>"
               : null;
        }

    }
}