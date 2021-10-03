using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;

namespace Dryv.SampleVue.Models
{
    internal static class AddressValidation
    {
        public static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
            .Rule<AsyncValidator>(a => a.ZipCode, (a, v) => v.ValidateZipCode(a.ZipCode, a.City, 5));
    }

    [DryvValidation(typeof(AddressValidation))]
    public class Address
    {
        public string City { get; set; }

        public string ZipCode { get; set; }

        public AddressExtras Extra1 { get; set; }

        public AddressExtras Extra2 { get; set; }
    }

    internal static class AddressExtrasValidation
    {
        public static DryvRules<AddressExtras> ValidationRules = DryvRules.For<AddressExtras>()
            .Rule(a => a.Field1, a => a.Field1 != null && a.Field1.Contains("nowhere") ? "Cannot contain 'nowhere'." : null)
            .Rule(a => a.Field2, a => a.Field2 != null && a.Field2.Contains("nowhere") ? "Cannot contain 'nowhere'." : null);
    }


    [DryvValidation(typeof(AddressExtrasValidation))]
    public class AddressExtras
    {
        public string Field1 { get; set; }

        public string Field2 { get; set; }
    }
}