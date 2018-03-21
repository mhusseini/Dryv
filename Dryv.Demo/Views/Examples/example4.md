## Pre-evaluation of static properties and fields
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