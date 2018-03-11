using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dryv.Tests
{
    [TestClass]
    public class StringTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void TranslateEqualsWithIgnoreCase()
        {
            var expression = Expression(m =>
                m.Text.Equals("Oscorp", StringComparison.OrdinalIgnoreCase)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var binaryExpression = conditional.Test as BinaryExpression;

            var leftMethod = GetMethod(binaryExpression?.Left);
            Assert.AreEqual("toLowerCase", leftMethod.Name);

            var rightMethod = GetMethod(binaryExpression?.Right);
            Assert.AreEqual("toLowerCase", rightMethod.Name);
        }

        [TestMethod]
        public void TranslateStartsWithWithIgnoreCase()
        {
            var expression = Expression(m =>
                m.Text.StartsWith("Oscorp", StringComparison.OrdinalIgnoreCase)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var binaryExpression = conditional.Test as BinaryExpression;

            var leftMethod = GetMethod(binaryExpression?.Left);
            var leftMethod2 = GetMethod(leftMethod.Object);

            Assert.AreEqual("indexOf", leftMethod.Name);
            Assert.AreEqual("toLowerCase", leftMethod2.Name);
        }

        [TestMethod]
        public void TranslateEndsWithWithIgnoreCase()
        {
            var expression = Expression(m =>
                m.Text.EndsWith("Oscorp", StringComparison.OrdinalIgnoreCase)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var binaryExpression = conditional.Test as BinaryExpression;

            var leftMethod = GetMethod(binaryExpression?.Left);
            var leftMethod2 = GetMethod(leftMethod.Object);

            Assert.AreEqual("indexOf", leftMethod.Name);
            Assert.AreEqual("toLowerCase", leftMethod2.Name);
        }

        [TestMethod]
        public void TranslateIndexOfWithIgnoreCase()
        {
            var expression = Expression(m =>
                m.Text.IndexOf("Oscorp", StringComparison.OrdinalIgnoreCase) == 0
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var binaryExpression = conditional.Test as BinaryExpression;

            var leftMethod = GetMethod(binaryExpression?.Left);
            var leftMethod2 = GetMethod(leftMethod.Object);

            Assert.AreEqual("indexOf", leftMethod.Name);
            Assert.AreEqual("toLowerCase", leftMethod2.Name);
        }

        [TestMethod]
        public void TranslateCompareTo()
        {
            var expression = Expression(m =>
                m.Text.CompareTo("Oscorp") == 0
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var binaryExpression = conditional.Test as BinaryExpression;

            var leftMethod = GetMethod(binaryExpression?.Left);
            Assert.AreEqual("localeCompare", leftMethod.Name);
        }

        [TestMethod]
        public void TranslateStaticCompareTo()
        {
            var expression = Expression(m =>
                string.Compare(m.Text, "Oscorp", StringComparison.CurrentCulture) == 0
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var binaryExpression = conditional.Test as BinaryExpression;

            var leftMethod = GetMethod(binaryExpression?.Left);
            Assert.AreEqual("localeCompare", leftMethod.Name);
        }

        [TestMethod]
        public void TranslateStaticCompareToCaseInsensitive()
        {
            var expression = Expression(m =>
                string.Compare(m.Text, "Oscorp", StringComparison.OrdinalIgnoreCase) == 0
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var binaryExpression = conditional.Test as BinaryExpression;

            var leftMethod = GetMethod(binaryExpression?.Left);
            var leftMethod2 = GetMethod(leftMethod.Object);

            Assert.AreEqual("toLowerCase", leftMethod2.Name);
            Assert.AreEqual("localeCompare", leftMethod.Name);
        }
    }
}