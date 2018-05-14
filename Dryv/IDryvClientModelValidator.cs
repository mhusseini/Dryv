using System;
using System.Collections.Generic;
using System.Reflection;
using Dryv.Configuration;

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
        /// <param name="property">The property for which the rules are created.</param>
        /// <param name="rules">The validation rules to be written to the client.</param>
        IDictionary<string, string> GetValidationAttributes(
            Type modelType,
            string modelPath,
            PropertyInfo property,
            Func<Type, object> services,
            DryvOptions options);
    }
}