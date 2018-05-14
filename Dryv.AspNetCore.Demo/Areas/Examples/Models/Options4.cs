namespace DryvDemo.Areas.Examples.Models
{
    public class Options4
    {
        public static string CompanyError { get; set; } = "The company name must be specified.";

        public const string CompanyLengthError = "The company name must be at least {0} characters long";

        public static int CompanyNameLength = 3;
    }
}