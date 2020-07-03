using System.Text.RegularExpressions;

namespace Dryv.SampleVue.CustomValidation
{
    public class SyncValidator
    {
        public DryvResultMessage ValidateName(string name)
        {
            return Regex.IsMatch(name, @"\d")
                ? "The name cannot contain digits."
                : null;
        }
    }
}