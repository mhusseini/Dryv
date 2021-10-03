<img src="logo_slogan.svg" title="Dryv - DRY Validation for ASP.NET MVC and ASP.NET Core" width="300">

<br/>
<br/>

[![NuGet version](https://badge.fury.io/nu/dryv.svg)](https://badge.fury.io/nu/dryv) 

**Complex model validation for server and client made easy.**

Write complex model validation rules in C# once.
_Dryv_ will generate JavaScript for client-side validation.

## Features
1. Automatically translates validation expressions from C# to JavaScript.
2. Supports asynchronous validation expressions.
3. Supports dependency-injection into validation expressions (e.g. for ASP.NET options).
4. Automatically generates ASP.NET controllers at runtime for custom async validation rules.
5. Integrates into the ASP.NET model validation. 

## Installation

### .NET Standard 1.2
```
Install-Package Dryv
```

### ASP.NET Core 3.1
```
Install-Package Dryv.AspNetCore 
```

## Usage
### .NET Standard 1.2
```csharp
using Dryv.Rules;

public class Address
{
    private static DryvRules<Address> ValidationRules = DryvRules.For<Address>()
        .Rule(a => a.City, a => string.IsNullOrWhiteSpace(a.City) ? "Please enter a city." : null)
        .Rule(a => a.ZipCode, a => string.IsNullOrWhiteSpace(a.ZipCode) ? "Please enter a ZIP code." : null)
        .Rule(a => a.ZipCode, a => a.ZipCode.Trim().Length < 5 ? "ZIP code must have at least 5 characters." : null)
        .Rule<AsyncValidator, SampleOptions>(a => a.ZipCode, (a, v, o) => v.ValidateZipCode(a.ZipCode, a.City, o.Strict));

    [DryvRules]
    public string City { get; set; }

    [DryvRules]
    public string ZipCode { get; set; }
}

public class HomeModel
{
    public Address BillingAddress { get; set; }
    public Address ShippingAddress { get; set; }
    public Person Person { get; set; }
}
```

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dryv.Compilation;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.Validation;
using Dryv.SampleConsole.Models;

internal class Program
{
    private static async Task Main()
    {
        var model = new HomeModel
        {
            // ..
        };

        var validator = new DryvValidator();
        var errors = await validator.Validate(model, Activator.CreateInstance);

        foreach (var error in from e in errors
                              from m in e.Message
                              select m.Type + " " + e.Path + ": " + m.Text)
        {
            Console.WriteLine(error);
        }
    }
}
```

### ASP.NET Core 3.1
```csharp
using Dryv.AspNetCore;
using Dryv.AspNetCore.DynamicControllers;
using Dryv.Configuration;
using Dryv.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        // ..

        app.UseDryv();

        // ..
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // ..

        services
            .AddMvc()
            .AddDryv(options => options.UseClientFunctionWriter<DryvAsyncClientValidationFunctionWriter>())
            .AddDryvDynamicControllers(options => options.HttpMethod = DryvDynamicControllerMethods.Get)
            .AddDryvPreloading();

        // ..
    }
}
```
```html
@model HomeModel

<!.. some HTML  ..>

<script>
    window.dryv = window.dryv || {};
    window.dryv.validators = @Html.Raw(string.Join(",\n", from val in Html.GetDryvClientValidationFunctions()
                                                          select $@"""{val.Key}"": {val.Value}"));
</script>
```
```javascript
window.dryv.validators = {
    validators: "billingAddress.zipCode": function(m, context) {
        return [
            function(a) {
                return (!/\S/.test(a.billingAddress.zipCode || "") ? "Please enter a ZIP code." : null);
            },
            function(a) {
                return ((a.billingAddress.zipCode.trim().length < 5) ? "ZIP code must have at least 5 characters." : null);
            },
            function(a, v, o) {
                return dryv.validateAsync('/validation/DryvDynamic1/ValidateZipCode', 'GET', {
                    zipCode: a.billingAddress.zipCode,
                    city: a.billingAddress.city
                });
            },
        ].reduce(function(promiseChain, currentTask) {
            return promiseChain.then(function(error) {
                return error || currentTask(m, context);
            });
        }, Promise.resolve());
    },
    // ...
};
```