using System;
using Dryv.Extensions;
using Escape.Ast;
using Jurassic;
using Jurassic.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class StringTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void InterpolationStrings()
        {
            var expression = Expression(m => $"{m.Text}");

            var jsProgram = GetTranslatedAst(expression);
            var binaryExpression = (dynamic)GetBodyExpression<BinaryExpression>(jsProgram);
            Assert.AreEqual(BinaryOperator.Plus, binaryExpression.Operator);
            Assert.AreEqual(nameof(TestModel.Text).ToCamelCase(), binaryExpression.Left.Right.Property.Name);
        }

        [TestMethod]
        public void TranslateCompareTo()
        {
            var expression = Expression(m =>
                m.Text.CompareTo("Oscorp") == 0
                    ? "fail"
                    : DryvValidationResult.Success);

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
                m.Text.EndsWith("xy", StringComparison.OrdinalIgnoreCase)
                    ? "fail"
                    : DryvValidationResult.Success);

            var translation = Translate(expression);
            var model = @"{text:'zzzzzzzXY'}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script) as string;

            Assert.AreEqual("fail", result);
        }

        [TestMethod]
        public void TranslateNotEndsWith()
        {
            var expression = Expression(m =>
                m.Text.EndsWith("xy")
                    ? "fail"
                    : DryvValidationResult.Success);

            var translation = Translate(expression);
            var model = @"{text:'ab'}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual(Null.Value, result);
        }

        [TestMethod]
        public void TranslateEqualsWithIgnoreCase()
        {
            var expression = Expression(m =>
                m.Text.Equals("Oscorp", StringComparison.OrdinalIgnoreCase)
                    ? "fail"
                    : DryvValidationResult.Success);

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
                    : DryvValidationResult.Success);

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
                    : DryvValidationResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = (dynamic)GetBodyExpression<ConditionalExpression>(jsProgram);
            var unaryExpression = conditional.Test;
            var callExpression = unaryExpression.Argument;

            Assert.AreEqual(UnaryOperator.LogicalNot, unaryExpression.Operator);
            Assert.AreEqual("test", callExpression.Callee.Property.Name);
            Assert.AreEqual(@"/\S/", callExpression.Callee.Object.Raw);

            var logicalExpression = callExpression.Arguments[0];

            Assert.AreEqual(LogicalOperator.LogicalOr, logicalExpression.Operator);
            Assert.AreEqual(nameof(TestModel.Text).ToCamelCase(), logicalExpression.Left.Property.Name);
            Assert.AreEqual(string.Empty, logicalExpression.Right.Value);
        }

        [TestMethod]
        public void TranslateStartsWithWithIgnoreCase()
        {
            var expression = Expression(m =>
                m.Text.StartsWith("Oscorp", StringComparison.OrdinalIgnoreCase)
                    ? "fail"
                    : DryvValidationResult.Success);

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
                    : DryvValidationResult.Success);

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
                    : DryvValidationResult.Success);

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