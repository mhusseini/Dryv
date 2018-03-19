using Escape;
using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Dryv.Tests
{
    [TestClass]
    public class JavascriptTranslatorTests : JavascriptTranslatorTestsBase
    {
        private readonly string patternField = @"^\d+$";
        private readonly Regex regexField = new Regex(@"^\d+$");
        private readonly string var2Field = "fail";

        private string PatternProperty => this.patternField;
        private Regex RegexProperty => this.regexField;
        private string Var2Property => this.var2Field;

        [TestMethod]
        public void GetArgumentFromField()
        {
            var expression = Expression(m =>
                new Regex(this.patternField, RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(m.Text)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(this.patternField, regexp?.Pattern);
        }

        [TestMethod]
        public void GetArgumentFromFieldWhereOtherFieldExists()
        {
            var expression = Expression(m =>
                new Regex(this.patternField, RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(m.Text)
                    ? this.var2Field
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(this.patternField, regexp?.Pattern);
        }

        [TestMethod]
        public void GetArgumentFromProperty()
        {
            var expression = Expression(m =>
                new Regex(this.PatternProperty, RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(m.Text)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(this.PatternProperty, regexp?.Pattern);
        }

        [TestMethod]
        public void GetArgumentFromPropertyWhereOtherPropertyExists()
        {
            var expression = Expression(m =>
                new Regex(this.PatternProperty, RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(m.Text)
                    ? this.Var2Property
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(this.PatternProperty, regexp?.Pattern);
        }

        [TestMethod]
        public void GetArgumentFromVariable()
        {
            var pattern = @"^\d+$";
            var expression = Expression(m =>
                new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(m.Text)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(pattern, regexp?.Pattern);
        }

        [TestMethod]
        public void GetArgumentFromVariableWhereOtherVariableExists()
        {
            var pattern = @"^\d+$";
            var var2 = "fail";
            var expression = Expression(m =>
                new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(m.Text)
                    ? var2
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(pattern, regexp?.Pattern);
        }

        [TestMethod]
        public void GetObjectFromField()
        {
            var expression = Expression(m =>
                this.regexField.IsMatch(m.Text)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(this.regexField.ToString(), regexp?.Pattern);
        }

        [TestMethod]
        public void GetObjectFromFieldWhereOtherFieldExists()
        {
            var expression = Expression(m =>
                this.regexField.IsMatch(m.Text)
                    ? this.var2Field
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(this.regexField.ToString(), regexp?.Pattern);
        }

        [TestMethod]
        public void GetObjectFromProperty()
        {
            var expression = Expression(m =>
                this.RegexProperty.IsMatch(m.Text)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(this.RegexProperty.ToString(), regexp?.Pattern);
        }

        [TestMethod]
        public void GetObjectFromPropertyWhereOtherPropertyExists()
        {
            var expression = Expression(m =>
                this.RegexProperty.IsMatch(m.Text)
                    ? this.Var2Property
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(this.RegexProperty.ToString(), regexp?.Pattern);
        }

        [TestMethod]
        public void GetObjectFromVariable()
        {
            var pattern = @"^\d+$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var expression = Expression(m =>
                 regex.IsMatch(m.Text)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(pattern, regexp?.Pattern);
        }

        [TestMethod]
        public void GetObjectFromVariableWhereOtherVariableExists()
        {
            var pattern = @"^\d+$";
            var var2 = "fail";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var expression = Expression(m =>
                regex.IsMatch(m.Text)
                    ? var2
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(pattern, regexp?.Pattern);
        }
    }
}