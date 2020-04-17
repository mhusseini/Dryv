using Dryv.SampleVue.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dryv.SampleVue.Pages
{
    public class IndexModel : PageModel
    {
        public Person Person { get; set; }

        public Address ShippingAddress { get; set; }

        public Address BillingAddress { get; set; }
    }
}