using System.Threading.Tasks;

namespace Dryv.SampleVue.CustomValidation
{
    public class AsyncValidator
    {
        public Task<DryvResultMessage> ValidateZipCode(string zipCode, string city)
        {
            return Task.FromResult(DryvResultMessage.Error("Validator not implemented."));
        }
    }
}