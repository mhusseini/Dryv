using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Dryv.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv.MethodCallTranslation
{
    internal class DefaultMethodCallTranslator : MethodCallTranslator
    {
        private static readonly MemberInfo ErrorMember = typeof(DryvResult).GetMember("Error").First();
        private readonly DryvOptions options;

        public DefaultMethodCallTranslator(IOptions<DryvOptions> options)
        {
            this.options = options.Value;
        }

        public override IList<Regex> TypeMatches { get; } = new List<Regex>();
        protected override List<(string Method, Action<MethodTranslationParameters> Translator)> MethodTranslators { get; } = null;

        public override bool Translate(MethodTranslationParameters parameters)
        {
            var objectType = parameters.Expression.Method.DeclaringType.FullName;
            var translator = this.options.MethodCallTanslators
                .FirstOrDefault(t => t.TypeMatches.Any(r => r.IsMatch(objectType)));

            if (translator != null)
            {
                return translator.Translate(parameters);
            }

            switch (parameters.Expression.Method.Name)
            {
                case nameof(DryvResult.Error):
                    if (parameters.Expression.Method != ErrorMember)
                    {
                        throw new MethodCallNotAllowedException(parameters.Expression);
                    }

                    parameters.Writer.Write(parameters.Expression.Arguments.First());
                    return true;

                default:
                    throw new MethodCallNotAllowedException(parameters.Expression);
            }
        }
    }
}