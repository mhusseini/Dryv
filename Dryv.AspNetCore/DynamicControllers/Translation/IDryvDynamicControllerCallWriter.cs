using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Translation;

namespace Dryv.DynamicControllers.Translation
{
    public interface IDryvDynamicControllerCallWriter
    {
        void Write(CustomTranslationContext context, string urlPlaceHolder, Dictionary<ParameterInfo, Expression> parameters);
    }
}