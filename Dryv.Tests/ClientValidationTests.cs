using System;
using System.Linq;
using System.Linq.Expressions;
using Dryv.AspNetCore;
using Dryv.Configuration;
using Dryv.Translation;
using Dryv.Validation;
using Microsoft.Extensions.Options;
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
            translatorMock.Setup(m => m.Translate(It.IsAny<Expression>(), It.IsAny<MemberExpression>(), It.IsAny<string>()))
                .Returns(() => new TranslationResult
                {
                    Factory = (_, __) => "nop",
                    CodeTemplate = "nothing",
                    OptionTypes = new Type[0]
                });
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(p => p.GetService(It.Is<Type>(t => t == typeof(ITranslator))))
                .Returns(translatorMock.Object);

            var validator = new DryvClientValidationFunctionWriter();
            var loader = new DryvClientValidationLoader(validator, Options.Create(new DryvOptions()), serviceProvider.Object);
            var results = loader.GetDryvClientValidation<ParentModel>();
            var l = results.ToList();

            Assert.AreEqual(3, l.Count);
        }

        private abstract class ChildModel
        {
            private static readonly DryvRules Rules = DryvRules.For<ChildModel>().Rule(m => m.Name, m => m.Name == "fail2" ? "invalid" : null);

            public GrandChildModel Child { get; set; }

            [DryvRules]
            public string Name { get; set; }
        }

        private abstract class GrandChildModel
        {
            private static readonly DryvRules Rules = DryvRules.For<GrandChildModel>().Rule(m => m.Name, m => m.Name == "fail3" ? "invalid" : null);

            [DryvRules]
            public string Name { get; set; }
        }

        private abstract class ParentModel
        {
            private static readonly DryvRules Rules = DryvRules.For<ParentModel>()
                .Rule(m => m.Name, m => m.Name == "fail1" ? "invalid" : null)
                .Rule(m => m.Child.Child.Name, m => m.Name == "fail4" ? "invalid" : null);

            public ChildModel Child { get; set; }

            [DryvRules]
            public string Name { get; set; }
        }
    }
}