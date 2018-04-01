using Dryv.Translation;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.DependencyInjection
{
    /// <summary>
    /// An interface for configuring Dryv services.
    /// </summary>
    public interface IDryvBuilder
    {
        /// <summary>
        /// The service collection in which the Dryv services are registered.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Registers a translator type.
        /// </summary>
        /// <typeparam name="T">The type of the translator to register.</typeparam>
        /// <returns>The registered translator may implement <see cref="IMethodCallTranslator"/> 
        /// or <see cref="ICustomTranslator"/>.</returns>
        IDryvBuilder AddTranslator<T>();

        /// <summary>
        /// Registers a translator object.
        /// </summary>
        /// <param name="translator">The type of the translator to register.</param>
        /// <returns>The registered translator may implement <see cref="IMethodCallTranslator"/> 
        /// or <see cref="ICustomTranslator"/>.</returns>
        IDryvBuilder AddTranslator(object translator);
    }
}