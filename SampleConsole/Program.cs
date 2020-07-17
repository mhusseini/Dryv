using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dryv.Configuration;
using Dryv.Rework;
using Dryv.Rework.Compilation;
using Dryv.Rework.RuleDetection;
using Dryv.RuleDetection;
using Dryv.Validation;
using Dryv.SampleConsole.Models;

internal class Program
{
    private static async Task Main()
    {
        var model = new HomeModel
        {
            Person = new Person(),
            ShippingAddress = new Address(),
            BillingAddress = new Address { Deactivated = true },
        };

        var options = new DryvOptions();
        var validator = new DryvValidator(new DryvRuleFinder(new ModelTreeBuilder(), new DryvCompiler(), null, options));

        var errors = await validator.Validate(model, Activator.CreateInstance);

        foreach (var error in from e in errors
                              select e.Value.Type + " " + e.Key + ": " + e.Value.Text)
        {
            Console.WriteLine(error);
        }
    }
}