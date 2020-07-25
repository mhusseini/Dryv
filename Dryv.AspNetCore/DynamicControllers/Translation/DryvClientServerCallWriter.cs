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
        public void Write(TranslationContext context, ITranslator translator, string url, string httpMethod, IList<MemberExpression> members)
        {
            var w = context.Writer;

            w.Write("dryv.validateAsync('");
            w.Write(url);
            w.Write("','");
            w.Write(httpMethod);
            w.Write("',");

            var parameter = members
                .SelectMany(ExpressionNodeFinder<ParameterExpression>.FindChildrenStatic)
                .First(p => p.Type == context.ModelType);
            var visitor = new ObjectWriter(translator, context, members.ToDictionary(m => m.Member, m => (Expression)m), w);
            visitor.Write(parameter.Type);

            w.Write(@").then(function($result){ return $context && $context.intercept ? $context.intercept($context, ");
            w.Write(parameter.Name);
            w.Write(", ");
            w.Write(context.Translator.TranslateValue(context.Rule.ModelPath));
            w.Write(", ");
            w.Write(context.Translator.TranslateValue(context.Rule.Name));
            w.Write(", $result) : $result;})");
        }

        private class ObjectWriter
        {
            private IDictionary<MemberInfo, Expression> members;
            private readonly ITranslator translator;
            private readonly TranslationContext context;
            private TextWriter writer;
            private Dictionary<Expression, List<Expression>> usedObjects;

            public ObjectWriter(ITranslator translator, TranslationContext context, IDictionary<MemberInfo, Expression> members, TextWriter writer)
            {
                this.usedObjects = (from expression in members.Values
                                    from outer in expression.GetOuterExpressions<ParameterExpression>().Select(containingExpression => (expression, containingExpression))
                                    group outer.containingExpression by outer.expression)
                    .ToDictionary(g => g.Key, g => g.ToList());

                this.members = members;
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
                    if (!this.members.TryGetValue(member, out var expression))
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
        }
    }
}