using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Translation;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    public interface IDryvClientServerCallWriter
    {
        void Write(CustomTranslationContext context, string url, string httpMethod, Dictionary<ParameterInfo, Expression> parameters);
    }
}