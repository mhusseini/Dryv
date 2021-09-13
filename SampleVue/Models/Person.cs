using System;
using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;

namespace Dryv.SampleVue.Models
{
    [DryvValidation]
    public class Person
    {
        private static DryvRules<Person> ValidationRules = DryvRules.For<Person>()
                .Rule(m => m.FirstName, m => string.IsNullOrWhiteSpace(m.FirstName) ? "Please provide a firstname.": null)
                .Rule(m => m.LastName, m => string.IsNullOrWhiteSpace(m.LastName) ? "Please provide a lastname." : null);

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}