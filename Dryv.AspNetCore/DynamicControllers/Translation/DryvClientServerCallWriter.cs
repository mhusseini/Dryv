using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.AspNetCore.Extensions;
using Dryv.Extensions;
using Dryv.Translation;
using Dryv.Translation.Visitors;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvClientServerCallWriter : IDryvClientServerCallWriter
    {
        public void Write(TranslationContext context, ITranslator translator, string url, string httpMethod, IList<MemberExpression> modelMembers)
        {
            WritePre(context, url, httpMethod);

            var parameter = modelMembers
                .SelectMany(ExpressionNodeFinder<ParameterExpression>.FindChildrenStatic)
                .First(p => p.Type == context.ModelType);
            
            var visitor = new ObjectWriter(translator, context, modelMembers.ToDictionary(m => m.Member, m => (Expression)m), context.Writer);
            visitor.Write(parameter.Type);

            WritePost(context, parameter);
        }
        
        public void Write(TranslationContext context, ITranslator translator, string url, string httpMethod, ParameterExpression modelParameter)
        {
            WritePre(context, url, httpMethod);
            WritePost(context, modelParameter);
        }

        public void Write(TranslationContext context, ITranslator translator, string url, string httpMethod)
        {
            WritePre(context, url, httpMethod);
            WritePost(context, null);
        }

        private static void WritePost(TranslationContext context, ParameterExpression parameter)
        {
            var w = context.Writer;

            w.Write(@").then(function($r){return $ctx.dryv.handleResult($ctx,");
            w.Write(parameter?.Name ?? "{}");
            w.Write(",");
            w.Write(context.Translator.TranslateValue(context.Rule.ModelPath));
            w.Write(",");
            w.Write(context.Translator.TranslateValue(context.Rule.Name));
            w.Write(",$r);})");
        }

        private static void WritePre(TranslationContext context, string url, string httpMethod)
        {
            var w = context.Writer;

            w.Write("$ctx.dryv.callServer('");
            w.Write(url);
            w.Write("','");
            w.Write(httpMethod);
            w.Write("',");
        }

        private class ObjectWriter
        {
            private readonly TranslationContext context;
            private readonly IDictionary<string, Expression> members;
            private readonly ITranslator translator;
            private readonly Dictionary<Expression, List<Expression>> usedObjects;
            private readonly TextWriter writer;

            public ObjectWriter(ITranslator translator, TranslationContext context, IDictionary<MemberInfo, Expression> members, TextWriter writer)
            {
                this.usedObjects = (
                        from expression in members.Values
                        from outer in expression.GetOuterExpressions<ParameterExpression>().Select(containingExpression => (expression, containingExpression))
                        group outer.containingExpression by outer.expression)
                    .ToDictionary(g => g.Key, g => g.ToList());

                this.members = members.ToDictionary(
                    m => GetMemberKey(m.Key),
                    m => m.Value,
                    StringComparer.OrdinalIgnoreCase);

                this.translator = translator;
                this.context = context;
                this.writer = writer;
            }

            public void Write(Type type)
            {
                var sep = string.Empty;

                this.writer.Write("{");

                foreach (var member in type.GetPropertiesAndFields())
                {
                    if (!this.members.TryGetValue(GetMemberKey(member), out var expression))
                    {
                        continue;
                    }

                    this.writer.Write(sep);
                    this.writer.Write("\"");
                    this.writer.Write(member.Name.ToCamelCase());
                    this.writer.Write("\"");
                    this.writer.Write(":");

                    if (member.IsNavigationMember())
                    {
                        if (this.usedObjects.ContainsKey(expression))
                        {
                            this.Write(member.GetMemberType());
                        }
                    }
                    else
                    {
                        this.translator.Translate(expression, this.context);
                    }

                    sep = ",";
                }

                this.writer.Write("}");
            }

            private static string GetMemberKey(MemberInfo member) => member.DeclaringType.FullName + ":" + member.Name;
        }
    }
}