using Dryv.DependencyInjection;
using Dryv.MethodCallTranslation;
using Dryv.Translation;
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

        protected static System.Linq.Expressions.Expression<Func<TModel, DryvResult>> Expression<TModel>(System.Linq.Expressions.Expression<Func<TModel, DryvResult>> exp) =>
            exp;

        protected static T GetBodyExpression<T>(FunctionExpression jsProgram)
            where T : Expression =>
            ((jsProgram.Body as BlockStatement)?.Body.First() as ReturnStatement)?.Argument as T;

        protected static (Expression Object, string Name) GetMethod(Expression expression)
        {
            var member = (expression as CallExpression)?.Callee as MemberExpression;
            return (Object: member?.Object, Name: (member?.Property as Identifier)?.Name);
        }

        protected static FunctionExpression GetTranslatedAst(
            System.Linq.Expressions.Expression expression,
            object[] translators = null,
            object[] validationOptions = null)
        {
            var translatorProvider = new TranslatorProvider();

            translatorProvider.MethodCallTranslators.Add(new RegexTranslator());
            translatorProvider.MethodCallTranslators.Add(new DryvResultTranslator());
            translatorProvider.MethodCallTranslators.Add(new StringTranslator());
            translatorProvider.GenericTranslators.Add(new RegexTranslator());
            translatorProvider.GenericTranslators.Add(new DryvResultTranslator());

            if (translators != null)
            {
                translatorProvider.MethodCallTranslators.AddRange(translators.OfType<IMethodCallTranslator>());
                translatorProvider.GenericTranslators.AddRange(translators.OfType<IGenericTranslator>());
            }

            var translator = new JavaScriptTranslator(
                new DefaultTranslator(translatorProvider),
                translatorProvider);

            var translation = translator.Translate(expression).Factory(validationOptions);
            var jsParser = new JavaScriptParser();
            return jsParser.ParseFunctionExpression(translation);
        }

        protected abstract class TestModel
        {
            public abstract string Text { get; set; }
        }
    }
}