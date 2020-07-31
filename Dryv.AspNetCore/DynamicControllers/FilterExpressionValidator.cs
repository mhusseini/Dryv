using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.Rules;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.AspNetCore.DynamicControllers
{
    public static class FilterExpressionValidator
    {
        private class DummyModel
        {
            public string DummyProperty { get; set; }
        }

        public static void ValidateFilterExpressions(Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> filters)
        {
            throw new NotImplementedException();
            //var dummyRule = DryvRules.For<DummyModel>().Rule(m => m.DummyProperty, m => DryvValidationResult.Success);
            //var dummyContext = new DryvControllerGenerationContext(typeof(TestControllerDummy), nameof(TestControllerDummy), dummyRule);

            //foreach (var expression in from filter in filters(dummyContext)
            //                           where filter != null
            //                           select filter)
            //{
            //    ControllerFilterHelper.GetAttributeBuilderArgs(expression);
            //}
        }

        private class TestControllerDummy
        {
            public IActionResult Test(string test) { throw new NotSupportedException(); }
        }
    }
}