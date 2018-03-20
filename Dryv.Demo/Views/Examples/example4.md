## Localized strings
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