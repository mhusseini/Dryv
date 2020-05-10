using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dryv.AspNetCore;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Translation;
using Dryv.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dryv.Tests
{
    [TestClass]
    public class ClientValidationTests
    {
        [TestMethod]
        public void Child_rules_are_included_in_HTML_extensions()
        {
            var translatorMock = new Mock<ITranslator>();
            translatorMock.Setup(m => m.Translate(It.IsAny<Expression>(), It.IsAny<MemberExpression>()))
                .Returns(() => new TranslationResult
                {
                    Factory = (_, __) => "nop",
                    CodeTemplate = "nothing",
                    OptionTypes = new Type[0]
                });
            Func<Type, object> services = type =>
            {
                if (type == typeof(ITranslator))
                {
                    return translatorMock.Object;
                }

                throw new NotSupportedException();
            };
            var options = new DryvOptions();

            var modelType = typeof(ParentModel);
            var validator = new DryvClientValidationProvider();
            var processedTypes = new Stack<string>();
            var results = HtmlExtensions.CollectClientValidation(modelType, null, null, validator, processedTypes, options, services);

            var l = results.ToList();

            Assert.AreEqual(3, l.Count);
        }

        private abstract class ParentModel
        {
            private static readonly DryvRules Rules = DryvRules.For<ParentModel>()
                .Rule(m => m.Name, m => m.Name == "fail1" ? "invalid" : null)
                .Rule(m => m.Child.Child.Name, m => m.Name == "fail4" ? "invalid" : null);

            [DryvRules]
            public string Name { get; set; }

            public ChildModel Child { get; set; }
        }

        private abstract class ChildModel
        {
            private static readonly DryvRules Rules = DryvRules.For<ChildModel>().Rule(m => m.Name, m => m.Name == "fail2" ? "invalid" : null);

            [DryvRules]
            public string Name { get; set; }

            public GrandChildModel Child { get; set; }
        }

        private abstract class GrandChildModel
        {
            private static readonly DryvRules Rules = DryvRules.For<GrandChildModel>().Rule(m => m.Name, m => m.Name == "fail3" ? "invalid" : null);

            [DryvRules]
            public string Name { get; set; }
        }
    }
}
