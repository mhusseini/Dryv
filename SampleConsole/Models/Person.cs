using System;
using Dryv.Rules;
using Dryv.SampleVue.CustomValidation;

namespace Dryv.SampleConsole.Models
{
    public class Person
    {
        private static DryvRules<Person> ValidationRules = DryvRules.For<Person>()
            .Rule(m => m.FirstName, m => string.IsNullOrWhiteSpace(m.FirstName) ? "Please enter first name." : null)
            .Rule(m => m.LastName, m => string.IsNullOrWhiteSpace(m.LastName) ? "Please enter last name." : null)
            .Rule(
                m => m.FirstName,
                m => m.LastName,
                m => m.FirstName.Equals(m.LastName, StringComparison.OrdinalIgnoreCase) ? $"First and last name cannot both be '{m.FirstName}'." : null);

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}