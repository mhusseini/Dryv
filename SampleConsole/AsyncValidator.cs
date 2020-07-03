using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dryv.SampleVue.CustomValidation
{
    public class AsyncValidator
    {
        public async Task<DryvValidationResult> ValidateZipCode(string zipCode, string city, int zipCodeLength)
        {
            return zipCode.Length > zipCodeLength
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
                        : "The ZIP code must only contain even numbers and zero.";
        }
    }
}