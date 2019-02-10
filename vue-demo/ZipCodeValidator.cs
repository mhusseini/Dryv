using System.Threading.Tasks;

namespace DryvDemo
{
    public class ZipCodeValidator
    {
        public async Task<bool> ValidateZipCode(string zip)
        {
            await Task.Delay(200);
            return !string.IsNullOrWhiteSpace(zip) && zip != "12345";
        }
    }
}