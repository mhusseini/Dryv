using System.Collections.Generic;
using System.Linq;
using Dryv.Translation;
using Dryv.Validation;

namespace Dryv.AspNetCore
{
    public class DryvScriptBlockGenerator : IDryvScriptBlockGenerator
    {
        public virtual string GetScriptBody(IDictionary<string, DryvClientValidationItem> validators)
        {
            return validators.Any()
               ? @"<script>
                    (function(w){
                        var a = w.dryv = w.dryv || {};
                        " + string.Concat(validators.Select(i => $"a.{i.Key} = {i.Value.ValidationFunction};")) + @";
                    })(window);
                </script>"
               : null;
        }

    }
}