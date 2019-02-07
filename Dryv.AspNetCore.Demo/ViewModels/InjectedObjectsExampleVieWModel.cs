using System;
using System.ComponentModel.DataAnnotations;
using Dryv;
using Microsoft.Extensions.Options;

namespace DryvDemo.ViewModels
{
    //
    // All calls to injected objects (here: InjectedObjectsExampleVieWModelOptions)
    // are executed on the server on each requests. The generated client-side code  
    // gets cached, but it also contains placeholders that are replaced by the
    // results of the calls to the injected object.
    //
    public class InjectedObjectsExampleVieWModel
    {
        public static readonly DryvRules Rules = DryvRules.For<InjectedObjectsExampleVieWModel>()
            //
            // The expression 'o.Value.CompanyPrefix' gets replaced with a placeholder in the generated
            // client-code. On each request, 'o' is injected using IoC and passed to the expression.
            // The result is inserted into the client-code right before the code is sent to the client.
            //
            .Rule<IOptions<InjectedObjectsExampleVieWModelOptions>>(m => m.Company,
                (m, o) => m.Company.StartsWith(o.Value.CompanyPrefix, StringComparison.OrdinalIgnoreCase)
                    ? DryvResultMessage.Success
                    : $"The company name must begin with '{o.Value.CompanyPrefix}'.")

            //
            // In fact, there is no limit on the complexity of the expressions used with injected objects,
            // since these expressions only get executed on the server and not translated to client-code.
            //
            .Rule<IOptions<InjectedObjectsExampleVieWModelOptions>>(m => m.Slogan,
                (m, o) => string.IsNullOrWhiteSpace(m.Slogan) || m.Slogan.EndsWith(o.GetSloganPostfix(), StringComparison.OrdinalIgnoreCase)
                    ? DryvResultMessage.Success
                    : o.GetSloganError());

        [Required]
        [DryvRules]
        public string Company { get; set; }

        [DryvRules]
        public string Slogan { get; set; }

    }
}