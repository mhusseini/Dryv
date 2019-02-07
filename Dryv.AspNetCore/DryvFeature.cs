using System.Collections.Generic;

namespace Dryv
{
    internal sealed class DryvFeature
    {
        public Dictionary<string, DryvClientPropertyValidation> PropertyValidations { get; } = new Dictionary<string, DryvClientPropertyValidation>();
    }
}