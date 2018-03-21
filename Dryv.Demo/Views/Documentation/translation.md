## Validation Rules
A validation rule is created for a specific type using the `DryvRules` class. A rule is made up by at least 2 components: 
the name of the properties that are validated and an expression that defines the cutual validation rule.
For example, see the following class:
```csharp
public class Model
{
    public string Company { get; set; }

    public string Name { get; set; }
}
```
To define a validation rule for the _Customer_'s Name property, the following expression is used:  
```csharp
DryvRules.For<Model>()
         .Rule(m => m.Company,
               m => DryvResult.Success);
```
Of course, the rule above is useless since it always validates positively. Let's add some validation:
```csharp
DryvRules.For<Model>()
         .Rule(m => m.Company,
               m => string.IsNullOrWhiteSpace(m.Company) && string.IsNullOrWhiteSpace(m.Company)
                    ? DryvResult.Error("Either name or company  must be specified")
                    : DryvResult.Success);
```
The rule above validates the _Company_ property only. To make it validate both _Company_ and _Name_, add the name property:
```csharp
DryvRules.For<Model>()
         .Rule(m => m.Company,
               m => m.Name,
               m => string.IsNullOrWhiteSpace(m.Company) && string.IsNullOrWhiteSpace(m.Company)
                    ? DryvResult.Error("Either name or company  must be specified")
                    : DryvResult.Success);
```
To actually activate this rule within the ASP.NET model validation, anotate the properties with the `DryvRulesAttribute` and add the rules to the model: 
```csharp
public class Model
{
    private static DryvRules MyRules = DryvRules
         .For<Model>()
         .Rule(m => m.Company,
               m => m.Name,
               m => string.IsNullOrWhiteSpace(m.Company) && string.IsNullOrWhiteSpace(m.Company)
                    ? DryvResult.Error("Either name or company  must be specified")
                    : DryvResult.Success);

    [DryvRules]
    public string Company { get; set; }
	
    [DryvRules]
    public string Name { get; set; }
}
```
The name and the accessor of the rule property don't matter (in the example above the private property's name is _MyRule_), but it needs to be static.
Also, many properties may contain rules. All rule properties are discovered, as long as their generic argument matches the class to be validated (in
the exampl above, it's _Model_). 
## Rule Result
A rule returns an object indicating whether the field is valid or not. Rules return objects of type `DryvResult`. 
### Success
For example, a rule that is always valid may look as following:
```csharp
DryvRules.For<Model>()
         .Rule(m => m.Company,
               m => DryvResult.Success);
```

For convenience, successfull validation may be indicated by retuning null:
```csharp
DryvRules.For<Model>()
         .Rule(m => m.Company,
               m => null);
```

### Error
A rule that is always invalid may look as following:
```csharp
DryvRules.For<Model>()
         .Rule(m => m.Company,
               m => DryvResult.Error("The company is invalid"));
```

Also, failed validation my be indicated by returning  the error message as a string: 
```csharp
DryvRules.For<Model>()
         .Rule(m => m.Company,
               m => "The company is invalid");
```

### Warning
Rules may return warnings. Warnings are curretly not supported in server-side validation of ASP.NET Core. Client-side support
depends on the validation framework used. Currently, only jQuery Unobtrusive Validation is supported by Dryv, which does not utilize warnings.
```csharp
DryvRules.For<Model>()
         .Rule(m => m.Company,
			   m => DryvResult.Warninf("The company is not really valid, but we'll acceot the value anyway"));
```

## JavaScript Translation
There are certain rules for translation to JavaScript.

- All logical expressions and program flow operations get translated to JavaScript. Reflection-related keyowrds like `as`, `is`, `typeof` or `nameof` are not supported.
- All model and option properties and fields are translated into JavaScript.
- All static fields and properties get pre-evaluated and the result value is inserted into the JavaScript code.
- Only methods that have dedicated translators are translated to JavaScript. Currently, methods for the types `String`, `Regex` and `DryvResult` are supported. For a listcomplete list of the supported methods of theses types, see the _compatibility_ section of this documentation.

## Option Pre-Evalutation 
Rules may contain options that are evaltuatied during translation time. That is, the value of these options will be inserted into the resulting JavaScript code instead of the expression itself.
```csharp
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
```JavaScript
function(m, o) { 
	return m.Company.indexOf("Awesome") === 0 
		? null 
		: "The company name must begin with '" + "Awesome" + "'."; 
}
```

## Rule switches
Rules can dynamically be turned on and off.
```csharp
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

## Static Member Pre-Evalutation 
Static properties and fields always are pre-evaluated and the resulting value is inserted into the JavaScript code. 
```csharp
public class Options4
{
    public static string CompanyError { get; set; } = "The company name must be specified.";

    public const string CompanyLengthError = "The company name must be at least {0} characters long";

    public static int CompanyNameLength = 3;
}

public class Model4
{
    public static readonly DryvRules Rules = DryvRules
        .For<Model4>()
        .Rule(m => m.Company,
            m => string.IsNullOrWhiteSpace(m.Company)
                ? Options4.CompanyError
                : m.Company.Length < Options4.CompanyNameLength
                    ? string.Format(Options4.CompanyLengthError, Options4.CompanyNameLength)
                    : DryvResult.Success);

    [DryvRules]
    public string Company { get; set; }
}
```
```JavaScript
function(m) { 
	return !/\S/.test(m.Company || "") 
		? "The company name must be specified." 
		: (
			(m.Company.Length<3) 
			? "The company name must be at least " + 3 + " characters long" 
			: null
		); 
}
```
## Reusing Rules
Dryv matches rules on base classes and interfaces. Validation rules specified for properties of interfaces or classes can be used to validate 
properties of child and implementation classes. Also, validation rules my be specified anywhere, as long as they are assigned to static fields or properties of 
the class to valdiate. Example:

```csharp
public interface IModel 
{
	string Company { get; set; }

	string Name { get; set; }
}

public class NamingRules 
{
    public readonly static readonly DryvRules Rules = DryvRules
         .For<IModel>()
         .Rule(m => m.Company,
              m => m.Name,
              m => string.IsNullOrWhiteSpace(m.Company) && string.IsNullOrWhiteSpace(m.Company)
                    ? DryvResult.Error("Either name or company  must be specified")
                    : DryvResult.Success);

}

public class NameLengthRules 
{
    public readonly static readonly DryvRules Rules = DryvRules
         .For<IModel>()
         .Rule(m => m => m.Name,
              m => string.IsNullOrWhiteSpace(m.Name) && m.Name.Length < 5
                    ? DryvResult.Error("The name must be at least 5 characters long")
                    : DryvResult.Success);

}

public class Model
{
    private static readonly NamingRules = NamingRules.Rules;
    private static readonly NameLengthRules = NameLengthRules.Rules;

    [DryvRules]
    public string Company { get; set; }
	
    [DryvRules]
    public string Name { get; set; }
}
```