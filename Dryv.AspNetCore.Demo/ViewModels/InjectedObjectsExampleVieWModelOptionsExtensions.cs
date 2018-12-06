using Microsoft.Extensions.Options;

namespace DryvDemo.ViewModels
{
    internal static class InjectedObjectsExampleVieWModelOptionsExtensions
    {
        /// <summary>
        /// This method is used to add some artificial complexity for the sake of the example.
        /// </summary>
        public static string GetSloganPostfix(this IOptions<InjectedObjectsExampleVieWModelOptions> options)
        {
            return options.Value.SloganPostfix;
        }

        /// <summary>
        /// This method is used to add some artificial complexity for the sake of the example.
        /// </summary>
        public static string GetSloganError(this IOptions<InjectedObjectsExampleVieWModelOptions> options)
        {
            return $"The slogan name must end with '{options.Value.SloganPostfix}'.";
        }
    }
}