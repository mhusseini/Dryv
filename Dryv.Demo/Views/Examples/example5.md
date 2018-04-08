## Tree-spanning validation rules 
```csharp
public class Model5
{
    public static readonly DryvRules Rules = DryvRules
        .For<Model5>()
        .Rule(m => m.Child.Name,
            m => !string.IsNullOrWhiteSpace(m.Name) &&
                    !string.IsNullOrWhiteSpace(m.Child.Name) &&
                    m.Name != m.Child.Name
                ? $"{nameof(Model6.Name)} of {nameof(Model6)} must be {m.Name}"
                : null);

    [DryvRules]
    [DisplayName("Name of Model5")]
    public string Name { get; set; }

    public Model6 Child { get; set; }
}

public class Model6
{
    public static readonly DryvRules Rules = DryvRules
        .For<Model6>()
        .Rule(m => m.Name, m => string.IsNullOrWhiteSpace(m.Name)
            ? $"{nameof(Model6.Name)} of {nameof(Model6)} must not be empty"
            : null);

    public static readonly DryvRules Rules2 = DryvRules
        .For<Model6>()
        .Rule(m => m.Child.Name, m => string.IsNullOrWhiteSpace(m.Child.Name)
            ? $"{nameof(Model7.Name)} of {nameof(Model7)} must not be empty"
            : null);

    [DryvRules]
    [DisplayName("Name of Model6")]
    public string Name { get; set; }

    public Model7 Child { get; set; }
}

public class Model7
{
    [DryvRules]
    [DisplayName("Name of Model7")]
    public string Name { get; set; }
}
```