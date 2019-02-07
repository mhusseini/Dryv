using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv
{
    internal class DryvModelValidatorProvider : IModelValidatorProvider
    {
        public void CreateValidators(ModelValidatorProviderContext context)
        {
            context.Results.Insert(0, new ValidatorItem
            {
                IsReusable = true,
                Validator = new DryvModelValidator()
            });
        }
    }
}