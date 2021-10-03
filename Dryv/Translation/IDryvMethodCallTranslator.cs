using System;
using System.Linq.Expressions;

namespace Dryv.Translation
{
    /// <summary>
    /// Defines translators that translate method calls (specified by a <see cref="MethodCallExpression"/>). 
    /// </summary>
    public interface IDryvMethodCallTranslator
    {
        /// <summary>
        /// Returns a value indicating whether the  specified type is supported by the translator.
        /// </summary>
        bool SupportsType(Type type);

        /// <summary>
        /// Translates the specified method call.
        /// </summary>
        /// <param name="context">An object that contains the <see cref="MethodCallExpression"/> to be translated as well as some context information.</param>
        /// <returns><c>true</c>, if the method call could be translated; otherwise, <c>false</c>.</returns>
        bool Translate(MethodTranslationContext context);
    }
}