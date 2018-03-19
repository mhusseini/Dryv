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