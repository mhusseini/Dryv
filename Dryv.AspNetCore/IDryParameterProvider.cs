using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dryv.AspNetCore
{
public interface IDryParameterProvider
{
    IReadOnlyDictionary<string, object> GetParameters(ActionExecutingContext context);
}
}