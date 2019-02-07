using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.Internal
{
    internal sealed class DryvFeature
    {
        public ConcurrentDictionary<ActionContext, Dictionary<object, Dictionary<string, List<DryvValidationResult>>>> CurrentValidationResults { get; } = new ConcurrentDictionary<ActionContext, Dictionary<object, Dictionary<string, List<DryvValidationResult>>>>();
        public Dictionary<string, DryvClientPropertyValidation> PropertyValidations { get; } = new Dictionary<string, DryvClientPropertyValidation>();
    }
}