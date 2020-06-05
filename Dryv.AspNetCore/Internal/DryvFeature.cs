using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dryv.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.AspNetCore.Internal
{
    internal sealed class DryvFeature
    {
        public ConcurrentDictionary<ActionContext, Task<Dictionary<object, Dictionary<string, List<DryvResult>>>>> CurrentValidationResults { get; } = new ConcurrentDictionary<ActionContext, Task<Dictionary<object, Dictionary<string, List<DryvResult>>>>>();
        public Dictionary<string, DryvClientValidationItem> PropertyValidations { get; } = new Dictionary<string, DryvClientValidationItem>();
    }
}