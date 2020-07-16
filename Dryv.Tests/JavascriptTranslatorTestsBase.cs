using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Rules;
using Dryv.Translation;
using Dryv.Translation.Translators;
using Escape;
using Escape.Ast;
using Expression = Escape.Ast.Expression;
using MemberExpression = Escape.Ast.MemberExpression;

namespace Dryv.Tests
{
    public class JavascriptTranslatorTestsBase
    {
        protected static System.Linq.Expressions.Expression<Func<TestModel, DryvValidationResult>> Expression(System.Linq.Expressions.Expression<Func<TestModel, DryvValidationResult>> exp) =>
            exp;

        protected static System.Linq.Expressions.Expression<Func<TModel, DryvValidationResult>> Expression<TModel>(System.Linq.Expressions.Expression<Func<TModel, DryvValidationResult>> exp) =>
            exp;

        protected static T GetBodyExpression<T>(FunctionExpression jsProgram)
            where T : Expression =>
            ((jsProgram.Body as BlockStatement)?.Body.First() as ReturnStatement)?.Argument as T;

        protected static (Expression Object, string Name) GetMethod(Expression expression)
        {
            var member = (expression as CallExpression)?.Callee as MemberExpression;
            return (member?.Object, (member?.Property as Identifier)?.Name);
        }

        protected static FunctionExpression GetTranslatedAst(
            System.Linq.Expressions.Expression expression,
            object[] translators = null,
            object[] validationOptions = null)
        {
            var translation = Translate(expression, translators, validationOptions);
            var jsParser = new JavaScriptParser();

            return jsParser.ParseFunctionExpression(translation);
        }

        protected static string Translate(System.Linq.Expressions.Expression expression, object[] translators = null, object[] validationOptions = null)
        {
            if (validationOptions == null)
            {
                validationOptions = new object[0];
            }

            var args = new object[] { "" }.Union(validationOptions).ToArray();
            var translator = CreateTranslator(translators);
            var translation = translator.Translate(expression, null, new DryvCompiledRule()).Factory(null, args);
            return translation;
        }

        private static JavaScriptTranslator CreateTranslator(object[] translators)
        {
            var translatorProvider = new TranslatorProvider();

            translatorProvider.MethodCallTranslators.Add(new RegexTranslator());
            translatorProvider.MethodCallTranslators.Add(new DryvValidationResultTranslator());
            translatorProvider.MethodCallTranslators.Add(new StringTranslator());
            translatorProvider.MethodCallTranslators.Add(new EnumerableTranslator());
            translatorProvider.GenericTranslators.Add(new RegexTranslator());
            translatorProvider.GenericTranslators.Add(new DryvValidationResultTranslator());
            translatorProvider.GenericTranslators.Add(new ObjectTranslator());

            if (translators != null)
            {
                translatorProvider.MethodCallTranslators.AddRange(translators.OfType<IMethodCallTranslator>());
                translatorProvider.GenericTranslators.AddRange(translators.OfType<ICustomTranslator>());
            }

            return new JavaScriptTranslator(translatorProvider, new DryvOptions());
        }

        protected abstract class TestModel
        {
            public bool BooleanValue { get; set; }
            public IEnumerable<int> IntItems { get; set; }
            public IEnumerable<string> Items { get; set; }
            public abstract string Text { get; set; }
        }
    }
}