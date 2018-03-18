namespace Dryv
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string text) => char.ToLowerInvariant(text[0]) + text.Substring(1);
    }
}