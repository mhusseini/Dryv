using Dryv.Translation;

namespace Dryv.Configuration
{
    /// <summary>
    /// An interface for configuring Dryv services.
    /// </summary>
    public interface IDryvBuilder<out TBuilder> where TBuilder : IDryvBuilder<TBuilder>
    {
        /// <summary>
        /// Registers a translator type.
        /// </summary>
        /// <typeparam name="T">The type of the translator to register.</typeparam>
        /// <returns>The registered translator may implement <see cref="IDryvMethodCallTranslator"/> 
        /// or <see cref="IDryvCustomTranslator"/>.</returns>
        TBuilder AddTranslator<T>();

        /// <summary>
        /// Registers a translator object.
        /// </summary>
        /// <param name="translator">The type of the translator to register.</param>
        /// <returns>The registered translator may implement <see cref="IDryvMethodCallTranslator"/> 
        /// or <see cref="IDryvCustomTranslator"/>.</returns>
        TBuilder AddTranslator(object translator);
    }
}