using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.AspNetCore.DynamicControllers
{
    public static class FilterExpressionValidator
    {
        public static void ValidateFilterExpressions(Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> filters)
        {
            var dummyRule = DryvRules.For<DummyModel>().Rule(m => m.DummyProperty, m => DryvValidationResult.Success).ValidationRules.First();
            var dummyContext = new DryvControllerGenerationContext(typeof(DummyController), nameof(DummyController.DummyAction), dummyRule);

            foreach (var expression in from filter in filters(dummyContext)
                                       where filter != null
                                       select filter)
            {
                ControllerFilterHelper.GetAttributeBuilderArgs(expression);
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private abstract class DummyModel
        {
            public string DummyProperty { get; set; }
        }

        private abstract class DummyController
        {
            public IActionResult DummyAction() => throw new NotSupportedException();
        }
    }
}