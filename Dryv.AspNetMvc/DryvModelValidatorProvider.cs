using System.Collections.Generic;
using System.Web.Mvc;

namespace Dryv.AspNetMvc
{
    internal class DryvModelValidatorProvider : ModelValidatorProvider
    {
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            yield return new DryvModelValidator(metadata, context);
        }
    }
}