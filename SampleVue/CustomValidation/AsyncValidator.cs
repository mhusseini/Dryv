using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dryv.SampleVue.CustomValidation
{
    public class AsyncValidator
    {
        public int GetLength(string text)
        {
            return text?.Length ?? 0;
        }

        public async Task<DryvValidationResult> ValidateZipCode(string zipCode, string city, int zipCodeLength)
        {
            return string.IsNullOrWhiteSpace(zipCode)
            ? "The ZIP code cannot be empty."
            : zipCode.Length > zipCodeLength
                ? $"The ZIP code cannot be longer than {zipCodeLength} characters."
                : await this.ValidateZipCode(zipCode, city);
        }

        public async Task<DryvValidationResult> ValidateZipCode(string zipCode, string city)
        {
            return string.IsNullOrWhiteSpace(zipCode)
                ? "ZIP code cannot be empty."
                : string.IsNullOrWhiteSpace(city)
                    ? "City cannot be empty."
                    : new Regex("^[02468]+$").IsMatch(zipCode)
                        ? null
                        : DryvValidationResult.Warning("The ZIP code should contain only zeros and even numbers. Are you sure you want to use it?");
        }
    }
}