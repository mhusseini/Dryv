using System.Linq.Expressions;

namespace Dryv.Translation
{
    /// <summary>
    /// Defines objects that are able to translate any given <see cref="Expression"/> to the client language.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Translates the specified expreeion.
        /// </summary>
        TranslationResult Translate(Expression expression, MemberExpression propertyExpression);
    }
}