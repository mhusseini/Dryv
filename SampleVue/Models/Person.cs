using System;
using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;

namespace Dryv.SampleVue.Models
{
    public class Person
    {
        //private static DryvRules<Person> ValidationRules = DryvRules.For<Person>()
        //    .Rule(m => m.FirstName, m => string.IsNullOrWhiteSpace(m.FirstName) ? "Please enter first name." : null)
        //    .Rule<SyncValidator>(m => m.FirstName, (m, v) => v.ValidateName(m.FirstName))
        //    .Rule(m => m.LastName, m => string.IsNullOrWhiteSpace(m.LastName) ? "Please enter last name." : null)
        //    .Rule<SyncValidator>(m => m.LastName, (m, v) => v.ValidateName(m.LastName))
        //    .Rule("name",
        //        m => m.FirstName,
        //        m => m.LastName,
        //        m => m.FirstName.Equals(m.LastName, StringComparison.OrdinalIgnoreCase) ? $"First and last name cannot both be '{m.FirstName}'." : null);

        [DryvValidation]
        public string FirstName { get; set; }

        [DryvValidation]
        public string LastName { get; set; }
    }
}