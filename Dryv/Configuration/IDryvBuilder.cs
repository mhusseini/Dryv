using Dryv.Translation;

namespace Dryv.Configuration
{
    /// <summary>
    /// An interface for configuring Dryv services.
    /// </summary>
    public interface IDryvBuilder<out TBuilder> where TBuilder : IDryvBuilder<TBuilder>
    {
        DryvOptions Options { get; }
    }
}