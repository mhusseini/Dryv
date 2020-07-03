using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dryv.AspNetCore
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DryvDisableAttribute : Attribute, IFilterMetadata
    {
    }
}