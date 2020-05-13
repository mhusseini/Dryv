using System;
using System.Linq;
using Dryv.Compilation;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.SampleConsole.Models;
using Dryv.Validation;

namespace Dryv.SampleConsole
{
    class Program
    {
        static void Main()
        {
            var model = new HomeModel
            {
                Person = new Person(),
                ShippingAddress = new Address(),
                BillingAddress = new Address { Deactivated = true },
            };

            var validator = new DryvValidator(new DryvRulesFinder(), new DryvServerRuleEvaluator());
            var errors = validator.Validate(model, new DryvOptions{BreakOnFirstValidationError = false}, Activator.CreateInstance);

            foreach (var error in from e in errors
                                  from m in e.Message
                                  select m.Type + " " + e.Path + ": " + m.Text)
            {
                Console.WriteLine(error);
            }
        }
    }
}
