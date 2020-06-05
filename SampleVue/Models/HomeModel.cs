using System.Threading.Tasks;

namespace Dryv.SampleVue.Models
{
    public class HomeModel
    {
        public bool BillingEqualsShipping { get; set; } = true;

        [DryvRules]
        public Address BillingAddress { get; set; }

        public Person Person { get; set; }

        public Address ShippingAddress { get; set; }
    }
}