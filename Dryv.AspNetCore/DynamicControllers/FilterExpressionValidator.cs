using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dryv.AspNetCore.DynamicControllers
{
    public static class FilterExpressionValidator
    {
        public static void ValidateFilterExpressions(Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> filters)
        {
            var dummyContext = new DryvControllerGenerationContext(typeof(TestControllerDummy), nameof(TestControllerDummy));

            foreach (var expression in from filter in filters(dummyContext)
                                       where filter != null
                                       select filter)
            {
                ControllerFilterHelper.GetAttributeBuilderArgs(expression);
            }
        }

        private class TestControllerDummy
        {
            public IActionResult Test(string test) { throw new NotSupportedException(); }
        }
    }
}