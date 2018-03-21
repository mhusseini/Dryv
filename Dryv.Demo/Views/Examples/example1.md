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
<form>
    <div class="form-group row">
        <label asp-for="Name" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <input asp-for="Name" class="form-control">
            <span asp-validation-for="Name"></span>
        </div>
    </div>

    <div class="form-group row">
        <label asp-for="Company" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <input asp-for="Company" class="form-control">
            <span asp-validation-for="Company"></span>
        </div>
    </div>

    <div class="form-group row">
        <label asp-for="TaxId" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <input asp-for="TaxId" class="form-control">
            <span asp-validation-for="TaxId"></span>
        </div>
    </div>

    <button class="btn btn-primary">Send</button>
</form>

```