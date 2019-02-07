using System.Threading.Tasks;

namespace DryvDemo
{
    public class ZipCodeValidator
    {
        public Task<bool> ValidateZipCode(string zip) => Task.FromResult(!string.IsNullOrWhiteSpace(zip) && zip != "12345");
    }
}