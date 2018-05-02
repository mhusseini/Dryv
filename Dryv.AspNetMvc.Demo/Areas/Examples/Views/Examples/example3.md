## Rule Switches
```csharp
public class Options
{
    public bool CompanyNameRequired { get; set; }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton(Options.Create(new Options3()));
	}
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