using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dryv.SampleVue.CustomValidation
{
    public class AsyncValidator
    {
        public async Task<DryvResultMessage> ValidateZipCode(string zipCode, string city)
        {
            if (string.IsNullOrWhiteSpace(zipCode)) return "ZIP code cannot be empty.";
            if (string.IsNullOrWhiteSpace(city)) return "City cannot be empty.";
            return new Regex("^[2468]+$").IsMatch(zipCode) ? null : "The ZIP code must only contain even numbers.";
        }
    }
}