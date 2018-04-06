using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv
{
    /// <summary>
    /// Defines objects that write validation rules to the client.
    /// </summary>
    public interface IDryvClientModelValidator
    {
        /// <summary>
        /// Translates the specified validation rules and adds them to the client.
        /// </summary>
        /// <param name="context">The context for client-side model validation.</param>
        /// <param name="rules">The validation rules to be written to the client.</param>
        void AddValidation(ClientModelValidationContext context, IEnumerable<DryvRuleNode> rules);
    }
}