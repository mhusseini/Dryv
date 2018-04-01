using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv
{
    public interface IDryvClientModelValidator
    {
        void AddValidation(ClientModelValidationContext context, IEnumerable<DryvRule> rules);
    }
}