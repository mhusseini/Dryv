# Dryv
 
[![NuGet version](https://badge.fury.io/nu/dryv.svg)](https://badge.fury.io/nu/dryv)

## Overview
According to [Rick Anderson](https://twitter.com/RickAndMSFT), "One of the design tenets of MVC is DRY ("Don't Repeat Yourself")" and 
"The validation support provided by MVC and Entity Framework Core Code First is a good example of the DRY principle in action. 
You can declaratively specify validation rules in one place (in the model class) and the rules are enforced everywhere in the app"  
([from Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app-xplat/validation)).

While this is the case for simple validation rules, applying complex validations rules is a different story. For instance, see the foloowing model.

```csharp
public class Customer
{
    [Required]
    public string Name { get; set; }

    public string Company { get; set; }

    public string TaxId { get; set; }
}
```

Using the annotation attributes provides by .NET, we can state that the `Name` property must be specified. ASP.NET MVC will render JavaScript 
code for us that performs model validation in the browser, and it will peroform server-side model validation in the controller. That's cool, certainly.
But consider the following case: neither the company nor the tax ID fields are required at first. But if the user enters a company name, the tax ID 
field becomes required. How would you implement such a validation? The recommended way is to write an own validation attribute subclassing 
[ValidationAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationattribute?view=netframework-4.7.1), 
make it implement [IClientModelValidator](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.validation.iclientmodelvalidator.addvalidation?view=aspnetcore-2.0),
implement server side validation and add client-side code to implement a jQuery validator. Real-world application can have lots and lots of different 
validation rules and implementing them in C# as well as JavaScript can become a cumbersome task.

That's where Dryv comes in. The name "Dryv" is derived from the term "DRY Validation".Using Dryv, you define the rules using C# expressions
and some inner magic will translate them to JavaScript. Taking teh example above, using Dryv it would look like this:

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

In the code above, a set of rules for the class `Customer` is defined. This set of rules contains a rule for the property `TaxId`.
The property `TaxId` has an attribute `DryvRulesAttributes` that makes Dryv play nicely with the ASP.NET MVC validation framework.
On the client, you need to load the appropriate JavaScript implementation of Dryv. Currently, an implementation exists for jQuery unobtrusive. Other 
implementations (e.g. for VueJS or React) can easily be added (and will be over time). 


## Installation
### Server
On the server, install the NuGet package:
```
dotnet add package Dryv 
```
... or ...
```
Install-Package Dryv 
```
... or ...
```
paket add Dryv 
```

### Client
On the client, install the NPM package:
```
npm install --save dryv-jquery-unobtrusive 
```
... or download the browser-specific JS file directly [from here](https://raw.githubusercontent.com/mhusseini/dryv-jquery-unobtrusive/master/dist/dryv-jquery-unobtrusive.browser.min.js) 
into your project and reference it from your page:
```
<script src="js/dryv-jquery-unobtrusive.browser.min.js"></script>
```
## Usage

In the ASP.NET Core startup class, add Dryv in the ConfigureServices method using the AddDryv extension method:

```csharp
using Dryv;

public class Startup
{
    // ...

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDryv()
                .AddMvc();
    }
}
```
Also in the startup class, use Dryv in the Configure method using the UseDryv extension method:

```csharp
using Dryv;

public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseDryv()
           .UseMvc();
    }

    // ...
}
```

# Examples
## Simple Example
```csharp
public class Customer
{
    public static readonly DryvRules Rules = DryvRules
        .For<Customer>()
        .Rule(m => m.TaxId,
            m => string.IsNullOrWhiteSpace(m.Company) || !string.IsNullOrWhiteSpace(m.TaxId)
                ? DryvResult.Success
                : $"The tax ID for {m.Company} must be specified.");

    public string Company { get; set; }

    [Required]
    public string Name { get; set; }

    [DryvRules]
    public string TaxId { get; set; }
}
```

```html
<form method="post">
    <div class="form-group">
        <label asp-for="Name"></label>
        <input asp-for="Name" class="form-control">
        <span asp-validation-for="Name"></span>
    </div>
    <div class="form-group">
        <label asp-for="Company"></label>
        <input asp-for="Company" class="form-control">
        <span asp-validation-for="Company"></span>
    </div>
    <div class="form-group">
        <label asp-for="TaxId"></label>
        <input asp-for="TaxId" class="form-control">
        <span asp-validation-for="TaxId"></span>
    </div>

    <button type="submit" class="btn btn-primary">Send</button>
</form>
```

## Pre-evaluated Options
Pre-evaluated options are objects that get evaluated prior to the actual validation phase. 
The pre-evaluated option values are then later used during model validation. For instance, 
the pre-evaluated option values are inserted into the generated client code, as opposed to a translated
expression as it would normally be the case. In the example below, the string `Awesome` will be
hard-coded into the generated JavaScript code.
 
```csharp
public class Options
{
    public string CompanyPrefix { get; set; } = "Awesome";
}

public class Model
{
    public static readonly DryvRules Rules = DryvRules
        .For<Model>()
        .Rule<IOptions<Options>>(m => m.Company,
            (m, o) => m.Company.StartsWith(o.Value.CompanyPrefix)
                ? DryvResult.Success
                : $"The company name must begin with '{o.Value.CompanyPrefix}'.");

    [DryvRules]
    public string Company { get; set; }
}
```

Objects used for pre-evaluation must be registered in the web applications service collection. That is,
they must be registered as a service in the startup class. For example:
```csharp
public class Startup
{
    // ...

    public void ConfigureServices(IServiceCollection services)
    {
        services.RegisterSingleton(Options.Create(new Options
        {
            CompanyPrefix = "Conglom-O"
        }));
    }
}
```

## Rule Switches
Using application options (registered as an application service like above), validation rules
can dynamically be activated or deactivated. In the example below, the validation rule will only
be applied if the value of `Options.CompanyNameRequired` is set to 'true'.
```csharp
public class Options
{
    public bool CompanyNameRequired { get; set; }
}

public class Model
{
    public static readonly DryvRules Rules = DryvRules
        .For<Model>()
        .Rule<IOptions<Options>>(m => m.Company,
            (m, o) => string.IsNullOrWhiteSpace(m.Company)
                ? "The company name must be specified."
                : DryvResult.Success,
            o => o.Value.CompanyNameRequired);

    [DryvRules]
    public string Company { get; set; }
}
```