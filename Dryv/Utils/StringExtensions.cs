namespace Dryv.Utils
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string text) => char.ToLowerInvariant(text[0]) + text.Substring(1);
        public static string ToPascalCase(this string text) => char.ToUpperInvariant(text[0]) + text.Substring(1);
    }
}