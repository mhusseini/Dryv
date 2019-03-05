using System;
using System.Reflection;
using Dryv.Configuration;

namespace Dryv.Validation
{
    /// <summary>
    /// Defines objects that write validation rules to the client.
    /// </summary>
    public interface IDryvClientValidationProvider
    {
        /// <summary>
        /// Translates the validation rules for the specified property to client code.
        /// </summary>
        /// <param name="property">The property for which the rules are created.</param>
        /// <param name="rules">The validation rules to be written to the client.</param>
        DryvClientPropertyValidation GetClientPropertyValidation(
            Type modelType,
            string modelPath,
            PropertyInfo property,
            Func<Type, object> services,
            DryvOptions options);
    }
}