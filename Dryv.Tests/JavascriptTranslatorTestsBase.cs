using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        protected static System.Linq.Expressions.Expression<Func<TestModel, DryvValidationResult>> Expression(System.Linq.Expressions.Expression<Func<TestModel, DryvValidationResult>> exp) =>
            exp;

        protected static System.Linq.Expressions.Expression<Func<TModel, DryvValidationResult>> Expression<TModel>(System.Linq.Expressions.Expression<Func<TModel, DryvValidationResult>> exp) =>
            exp;

        protected static System.Linq.Expressions.Expression<Func<object>> Expression(System.Linq.Expressions.Expression<Func<object>> exp) =>
            exp;

        
        protected static T GetBodyExpression<T>(FunctionExpression jsProgram)
            where T : Expression =>
            ((jsProgram.Body as BlockStatement)?.Body.First() as ReturnStatement)?.Argument as T;

        protected static (Expression Object, string Name) GetMethod(Expression expression)
        {
            var member = (expression as CallExpression)?.Callee as MemberExpression;
            return (member?.Object, (member?.Property as Identifier)?.Name);
        }

        protected static FunctionExpression GetTranslatedAst<TModel>(
            System.Linq.Expressions.Expression expression,
            object[] translators = null,
            object[] validationOptions = null)
        {
            var translation = Translate<TModel>(expression, translators, validationOptions);
            var jsParser = new JavaScriptParser();

            return jsParser.ParseFunctionExpression(translation);
        }


        protected static FunctionExpression GetTranslatedAst(
            System.Linq.Expressions.Expression expression,
            object[] translators = null,
            object[] validationOptions = null)
        {
            var translation = Translate<TestModel>(expression, translators, validationOptions);
            var jsParser = new JavaScriptParser();

            return jsParser.ParseFunctionExpression(translation);
        }

        protected static string Translate<TModel>(System.Linq.Expressions.Expression expression, object[] translators = null, object[] validationOptions = null)
        {
            if (validationOptions == null)
            {
                validationOptions = new object[0];
            }

            var rule = new DryvCompiledRule
            {
                ModelType = typeof(TModel)
            };

            var args = new object[] { "" }.Union(validationOptions).ToArray();
            var translator = CreateTranslator(translators);
            var translation = translator.Translate(expression, null, rule).Factory(null, args, new DryvOptions());
            return translation;
        }

        private static JavaScriptTranslator CreateTranslator(object[] translators)
        {
            var methodCallTranslators = new Collection<IDryvMethodCallTranslator>
            {
                new RegexTranslator(),
                new DryvValidationResultTranslator(),
                new StringTranslator(),
                new EnumerableTranslator()
            };

            var customTranslators = new Collection<IDryvCustomTranslator>
            {
                new RegexTranslator(),
                new DryvValidationResultTranslator(),
                new ObjectTranslator()
            };

            if (translators != null)
            {
                methodCallTranslators.AddRange(translators.OfType<IDryvMethodCallTranslator>());
                customTranslators.AddRange(translators.OfType<IDryvCustomTranslator>());
            }

            return new JavaScriptTranslator(customTranslators, methodCallTranslators, new DryvOptions
            {
                JsonConversion = v => JsonSerializer.Serialize(v, Options)
            });
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