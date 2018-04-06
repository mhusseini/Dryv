using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv.Mvc
{
    public class ObjectModelValidator : IObjectModelValidator
    {
        private readonly IModelMetadataProvider modelMetadataProvider;
        private readonly ValidatorCache validatorCache;
        private readonly IModelValidatorProvider validatorProvider;

        public ObjectModelValidator(
            IModelMetadataProvider modelMetadataProvider,
            IList<IModelValidatorProvider> validatorProviders)
        {
            if (validatorProviders == null)
            {
                throw new ArgumentNullException(nameof(validatorProviders));
            }

            this.modelMetadataProvider = modelMetadataProvider ?? throw new ArgumentNullException(nameof(modelMetadataProvider));
            this.validatorCache = new ValidatorCache();

            this.validatorProvider = new CompositeModelValidatorProvider(validatorProviders);
        }

        public void Validate(
            ActionContext actionContext,
            ValidationStateDictionary validationState,
            string prefix,
            object model)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var visitor = new ValidationVisitor(
                actionContext,
                this.validatorProvider,
                this.validatorCache,
                this.modelMetadataProvider,
                validationState);

            if (actionContext.HttpContext.RequestServices.GetService(typeof(IModelProvider)) is ModelProvider mp)
            {
                mp.SetModel(model);
            }

            var metadata = model == null ? null : this.modelMetadataProvider.GetMetadataForType(model.GetType());
            visitor.Validate(metadata, prefix, model);
        }
    }
}