using System;
using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class StringTests : JavascriptTranslatorTestsBase
    {
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
        public void TranslateIsNullOrWhiteSpace()
        {
            var expression = Expression(m =>
                string.IsNullOrWhiteSpace(m.Text)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = (dynamic)GetBodyExpression<ConditionalExpression>(jsProgram);
            var callExpression = conditional.Test;

            Assert.AreEqual("test", callExpression.Callee.Property.Name);
            Assert.AreEqual(@"/^\s$/", callExpression.Callee.Object.Raw);

            var logicalExpression = callExpression.Arguments[0];

            Assert.AreEqual(LogicalOperator.LogicalOr, logicalExpression.Operator);
            Assert.AreEqual(nameof(TestModel.Text), logicalExpression.Left.Property.Name);
            Assert.AreEqual(string.Empty, logicalExpression.Right.Value);
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