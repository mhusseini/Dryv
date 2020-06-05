using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.AspNetCore.DynamicControllers;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.Compilation;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.SampleConsole.Models;
using Dryv.SampleVue;
using Dryv.SampleVue.CustomValidation;
using Dryv.Validation;
using Microsoft.Extensions.Options;

namespace Dryv.SampleConsole
{
    class Program
    {
        static void Main()
        {
            Expression<Func<Address, AsyncValidator, SampleOptions, Task<DryvResultMessage>>> f = (a, v, o) => v.ValidateZipCode(a.ZipCode, a.City, o.ZipCodeLength + 1);

            var g = new ControllerGenerator(new OptionsWrapper<DryvDynamicControllerOptions>(new DryvDynamicControllerOptions
            {
                HttpMethod = DryvDynamicControllerMethods.Get
            }));

            var ass = g.CreateControllerAssembly(f.Body as MethodCallExpression, typeof(Address));
            var t = ass.GetTypes().First();
            var c = Activator.CreateInstance(t, new AsyncValidator(), new SampleOptions());
            var m = c.GetType().GetMethods().First(m => !m.Attributes.HasFlag(MethodAttributes.HideBySig));

            //var x = m.Invoke(c, new object[] { "1234", "Doooomcity" });
        }
    }
}
