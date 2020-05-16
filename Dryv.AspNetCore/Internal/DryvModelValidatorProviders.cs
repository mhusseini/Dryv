using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv.AspNetCore.Internal
{
    internal class DryvModelValidatorProvider : IModelValidatorProvider
    {
        public void CreateValidators(ModelValidatorProviderContext context)
        {
            context.Results.Add(new ValidatorItem
            {
                IsReusable = true,
                Validator = new DryvModelValidator(),
            });
        }
    }
}