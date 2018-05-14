## Pre-evaluated Options
```csharp
public class Options
{
    public string CompanyPrefix { get; set; } = "Awesome";
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton(Options.Create(new Options2()));
	}
}

public class Model
{
    public static readonly DryvRules Rules = DryvRules
        .For<Model>()
        .Rule<IOptions<Options>>(m => m.Company,
            (m, o) => m.Company.StartsWith(o.Value.CompanyPrefix)
                ? DryvResult.Success
                : $"The company name must begin with '{o.Value.CompanyPrefix}'.");

	[Required]
    [DryvRules]
    public string Company { get; set; }
}
```