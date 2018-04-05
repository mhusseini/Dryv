﻿using System.Linq.Expressions;

namespace Dryv.Translation
{
    /// <summary>
    /// Defines translators that perform a custom translation of any given <see cref="Expression"/>.
    /// </summary>
    public interface ICustomTranslator
    {
        /// <summary>
        /// Translates the specified <see cref="Expression"/>, if possible.
        /// </summary>
        /// <param name="context">An object that contains the <see cref="Expression"/> to be translated as well as soem context information.</param>
        /// <returns><c>true</c>, if the expression could be translated; otherwise, <c>false</c>.</returns>
        bool TryTranslate(CustomTranslationContext context);
    }
}