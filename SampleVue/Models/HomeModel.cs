namespace Dryv.SampleVue.Models
{
    [DryvValidation]
    public class HomeModel
    {
        // private static DryvRules Rules = DryvRules.For<HomeModel>()
        //     .DisableRules(m => m.BillingAddress, m => m.BillingEqualsShipping);

        public bool BillingEqualsShipping { get; set; } = true;

        public Address ShippingAddress { get; set; }

        public Address BillingAddress { get; set; }

        public Person Person { get; set; }
    }
}