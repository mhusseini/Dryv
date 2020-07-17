using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dryv.SampleVue.Models
{
    public class HomeModel
    {
        private static readonly Regex RegexPerson = new Regex(@"\d");
        private static DryvRules Rules = DryvRules.For<HomeModel>().DisableRules(m => m.BillingAddress, m => m.BillingEqualsShipping);

        private static DryvRules Rules2 = DryvRules.For<Address>().Rule(m => m.SomeEnum, m => m.SomeEnum == MyEnum.Value1 ? "Whaaaat?" : null);
        private static DryvRules Rules3 = DryvRules.For<Person>().Rule(m => m.LastName, m => RegexPerson.IsMatch(m.LastName) ? "Whaaaat?" : null);

        public bool BillingEqualsShipping { get; set; } = true;

        public Address BillingAddress { get; set; }

        public Person Person { get; set; }

        public Address ShippingAddress { get; set; }
    }
}