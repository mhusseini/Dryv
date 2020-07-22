using System.Linq.Expressions;

namespace Dryv.Translation
{
    /// <summary>
    /// Defines translators that perform a custom translation of any given <see cref="Expression"/>.
    /// </summary>
    public interface IDryvCustomTranslator
    {
        /// <summary>
        /// A number specifying the order in which tha translator is called. The higher the number, the later the translator will get called.
        /// </summary>
        int? OrderIndex { get; set; }

        /// <summary>
        /// Determines whether the calling translator may surround the translation of specified expression with round brackets "(" and ")".
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Must return <c>false</c> to disable surrounding brackets.</returns>
        bool? AllowSurroundingBrackets(Expression expression);

        /// <summary>
        /// Translates the specified <see cref="Expression"/>, if possible.
        /// </summary>
        /// <param name="context">An object that contains the <see cref="Expression"/> to be translated as well as soem context information.</param>
        /// <returns><c>true</c>, if the expression could be translated; otherwise, <c>false</c>.</returns>
        bool TryTranslate(CustomTranslationContext context);
    }
}