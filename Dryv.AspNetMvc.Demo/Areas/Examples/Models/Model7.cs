using System.ComponentModel;
using Dryv;

namespace DryvDemo.Areas.Examples.Models
{
    public class Model7
    {
        [DryvRules]
        [DisplayName("Name of Model7")]
        public string Name { get; set; }
    }
}