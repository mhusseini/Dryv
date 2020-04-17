<img src="logo_slogan.svg" title="Dryv - DRY Validation for ASP.NET MVC and ASP.NET Core" width="300">

[![NuGet version](https://badge.fury.io/nu/dryv.svg)](https://badge.fury.io/nu/dryv) 

**Complex model validation for server and client made easy.**

Write complex model validation rules in C# once.
_Dryv_ will generate JavaScript for client-side validation.

## Overview
According to [Rick Anderson](https://twitter.com/RickAndMSFT), 

> "One of the design tenets of MVC is DRY ("Don't Repeat Yourself")"

and

> "The validation support provided by MVC and Entity Framework Core Code First is a good example of the DRY principle in action. 
You can declaratively specify validation rules in one place (in the model class) and the rules are enforced everywhere in the app" ([from Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app-xplat/validation)).


While this is the case for simple validation rules, applying complex validations rules is a different story. For instance, see the following model.

```csharp
public class Customer
{
    [Required]
    public string Name { get; set; }

    public string Company { get; set; }

    public string TaxId { get; set; }
}
```

Using the annotation attributes provides by .NET, we can state that the `Name` property must be specified. ASP.NET MVC will render JavaScript code for us that performs model validation in the browser, and it will peroform server-side model validation in the controller. That's cool, certainly. 

Now consider the following case: neither the company nor the tax ID fields are required at first. But if the user enters a company name, the tax ID field becomes required. How would you implement such a validation?

The recommended approach **until now** is to write a custom validation attribute that inherits from [ValidationAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationattribute?view=netframework-4.7.1), make it implement [IClientModelValidator](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.validation.iclientmodelvalidator.addvalidation?view=aspnetcore-2.0), implement server side validation and add client-side code to implement a jQuery validator. Real-world application can have lots and lots of different validation rules and implementing them in C# as well as JavaScript can become a cumbersome task.

**That's where Dryv comes in.** The name "Dryv" is derived from the term "DRY Validation". Using Dryv, you define the rules using C# expressions and some inner magic will translate them to JavaScript. Using Dryv, the example above would look like this:

```csharp
public class Customer
{
    public static readonly DryvRules Rules = DryvRules
        .For<Customer>()
        .Rule(m => m.TaxId,
            m => string.IsNullOrWhiteSpace(m.Company) || !string.IsNullOrWhiteSpace(m.TaxId)
                ? DryvResult.Success
                : $"The tax ID for {m.Company} must be specified.");

    [Required]
    public string Name { get; set; }

    public string Company { get; set; }

    [DryvRules]
    public string TaxId { get; set; }
}
```

In the code above, a set of rules for the class `Customer` is defined. This set of rules contains a rule for the property `TaxId`. The property `TaxId` has an attribute `DryvRulesAttributes` that makes Dryv play nicely with the ASP.NET MVC validation framework. On the client, you need to load the appropriate JavaScript implementation of Dryv. Currently, an implementation exists for jQuery unobtrusive. Other implementations (e.g. for VueJS or React) can easily be added (and will be over time). 

## Installation
### ASP.NET Core
On the server, install the NuGet package:
```
Install-Package Dryv.AspNetCore 
```

### Stand-alone
In stand-alone (e.g. console) apps:
```
Install-Package Dryv
```

## Usage
### ASP.NET Core

In the ASP.NET Core startup class, add Dryv in the Configure ConfigureServices methods:

```csharp
using Dryv;

public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDryv();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc()
                .AddDryv();
        
        // or

        services.AddRazorPages()()
                .AddDryv();
    }
}
```

Afterwards, the validation results will be available via the [ASP.NET model state](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-3.1).

### Client-Side (JavaScript)
In your Razore view, you can output the translated validation expressions to the client. The exact format depends on your client implementation. See the VueJS sample in this repository for more details.
``` html
@model HomeModel

<script>
(function(dryv) {
    dryv.validators = {
        @Html.Raw(string.Join(",\n", from val in Html.GetDryvClientPropertyValidations()
                                     let field = val.Property.Name.ToCamelCase()
                                     let sep = string.IsNullOrWhiteSpace(val.ModelPath) ? string.Empty : "."
                                     select $@"""{val.ModelPath}{sep}{field}"": {val.ValidationFunction}"))
    };
})(window.dryv || (window.dryv = {}));
</script>
```

### Stand-alone
Dryv can be used without ASP.NET. Using just the base package, Dryv can be used to validate models.
```csharp
using System;
using Dryv;

namespace Demo
{
    internal class Program
    {
        private static void Main()
        {
            var model = new Model1
            {
                Name = "Hello",
                Child = new Model2
                {
                    Name = "World",
                    Child = new Model3()
                },
                Children = new[]
                {
                    new Model4()
                }
            };

            var validator = new DryvValidator(new DryvRulesFinder(new InMemoryCache()), new DryvServerRuleEvaluator());
            var errors = validator.Validate(model);

            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}
```



