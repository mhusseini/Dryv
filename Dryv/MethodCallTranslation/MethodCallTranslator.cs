using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dryv.MethodCallTranslation
{
    internal class MethodCallTranslator : MethodCallTranslatorBase
    {
        private static readonly MemberInfo ErrorMember = typeof(DryvResult).GetMember("Error").First();

        private readonly Dictionary<Type, MethodCallTranslatorBase> translators = new Dictionary<Type, MethodCallTranslatorBase>
        {
            [typeof(string)] = new StringMethodCallTranslator(),
            [typeof(Regex)] = new RegexMethodCallTranslator()
        };

        public override bool Translate(MethodTranslationOptions options)
        {
            if (this.translators.TryGetValue(options.Expression.Method.DeclaringType, out var translator))
            {
                return translator.Translate(options);
            }

            switch (options.Expression.Method.Name)
            {
                case nameof(DryvResult.Error):
                    if (options.Expression.Method != ErrorMember)
                    {
                        throw new MethodCallNotAllowedException(options.Expression);
                    }

                    options.Writer.Write(options.Expression.Arguments.First());
                    return true;

                default:
                    throw new MethodCallNotAllowedException(options.Expression);
            }
        }
    }
}