using System.Collections.Generic;
using System.Web.Mvc;

namespace Dryv.AspNetMvc
{
    internal class DryvModelClientValidationRule : ModelClientValidationRule
    {
        public DryvModelClientValidationRule(IDictionary<string, string> attributes)
        {
            this.ValidationType = "dryv";
            this.ErrorMessage = attributes[DryvClientModelValidator.DataValDryAttribute];
            this.ValidationParameters["type"] = attributes[DryvClientModelValidator.DataTypeDryAttribute];
        }
    }
}