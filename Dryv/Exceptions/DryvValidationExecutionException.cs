using System;
using Dryv.Rules;

namespace Dryv.Exceptions
{
    public class DryvValidationExecutionException : Exception
    {
        public DryvValidationExecutionException(DryvCompiledRule rule, Exception innerException) :
            base($"An error occurred while evaluating rule for '{GetPropertyPath(rule)}: {innerException.Message}'", innerException)
        {
        }

        private static string GetPropertyPath(DryvCompiledRule rule)
        {
            return string.IsNullOrWhiteSpace(rule.ModelPath)
                ? rule.Property.Name
                : $"{rule.ModelPath}.{rule.Property.Name}";
        }
    }
}