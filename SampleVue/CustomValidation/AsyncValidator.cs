using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dryv.SampleVue.CustomValidation
{
    public class AsyncValidator
    {
        public async Task<DryvResultMessage> ValidateZipCode(string zipCode, string city)
        {
            await Task.Delay(10);
            return new Regex("^[2468]+$").IsMatch(zipCode) ? null : "The ZIP code must only contain even numbers.";
        }
    }
}