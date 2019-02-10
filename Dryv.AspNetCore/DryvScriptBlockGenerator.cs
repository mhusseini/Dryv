using System.Linq;
using System.Collections.Generic;
using Dryv.Translation;
using Dryv.Validation;

namespace Dryv
{
    public class DryvScriptBlockGenerator : IDryvScriptBlockGenerator
    {
        public virtual string GetScriptBody(IDictionary<string, DryvClientPropertyValidation> validators)
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