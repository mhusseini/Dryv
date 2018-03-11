using Escape;
using Escape.Ast;
using System;
using System.Linq;

namespace Dryv.Tests
{
    public class JavascriptTranslatorTestsBase
    {
        protected static System.Linq.Expressions.Expression<Func<TestModel, DryvResult>> Expression(System.Linq.Expressions.Expression<Func<TestModel, DryvResult>> exp) =>
            exp;

        protected static T GetBodyExpression<T>(FunctionExpression jsProgram)
            where T : Expression =>
            ((jsProgram.Body as BlockStatement)?.Body.First() as ReturnStatement)?.Argument as T;

        protected static (Expression Object, string Name) GetMethod(Expression expression)
        {
            var member = (expression as CallExpression)?.Callee as MemberExpression;
            return (Object: member?.Object, Name: (member?.Property as Identifier)?.Name);
        }

        protected static FunctionExpression GetTranslatedAst(System.Linq.Expressions.Expression<Func<TestModel, DryvResult>> expression)
        {
            var translator = new JavaScriptTranslator();
            var translation = translator.Translate(expression);
            var jsParser = new JavaScriptParser();
            return jsParser.ParseFunctionExpression(translation);
        }

        protected abstract class TestModel
        {
            public abstract string Text { get; set; }
        }
    }
}