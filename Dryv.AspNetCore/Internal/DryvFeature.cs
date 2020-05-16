using System.Collections.Concurrent;
using System.Collections.Generic;
using Dryv.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.AspNetCore.Internal
{
    internal sealed class DryvFeature
    {
        public ConcurrentDictionary<ActionContext, Dictionary<object, Dictionary<string, List<DryvResult>>>> CurrentValidationResults { get; } = new ConcurrentDictionary<ActionContext, Dictionary<object, Dictionary<string, List<DryvResult>>>>();
        public Dictionary<string, DryvClientValidationItem> PropertyValidations { get; } = new Dictionary<string, DryvClientValidationItem>();
    }
}