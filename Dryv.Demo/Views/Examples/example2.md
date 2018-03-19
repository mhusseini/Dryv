## Pre-evaluated Options
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