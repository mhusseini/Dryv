using System.IO;
using System.Linq.Expressions;
using Dryv.Rules;

namespace Dryv.Translation
{
    /// <summary>
    /// Defines objects that are able to translate any given <see cref="Expression"/> to the client language.
    /// </summary>
    public interface ITranslator
    {
        string FormatIdentifier(string name);

        /// <summary>
        /// Translates the specified expression.
        /// </summary>
        TranslationResult Translate(Expression expression, MemberExpression propertyExpression, DryvCompiledRule rule);

        void Translate(Expression expression, TranslationContext context, bool negated = false);

        string TranslateValue(object value);

        bool TryWriteTerminal(Expression expression, TextWriter writer);
    }
}