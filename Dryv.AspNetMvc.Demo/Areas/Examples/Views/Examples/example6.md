## Boolean properties 
```csharp
public class Model8
{
    public static readonly DryvRules Rules = DryvRules
        .For<Model8>()
        .Rule(m => m.Name,
            m => !m.IsManly || m.Name.EndsWith("or")
                    ? DryvResult.Success
                    : DryvResult.Error("Overly manly names must end with 'or'."));

    [Required]
    [DryvRules]
    public string Name { get; set; }

    [DisplayName("Is overly manly")]
    public bool IsManly { get; set; }
}
```

```HTML
 <form id="form4" asp-controller="Examples" asp-action="Example5" onsubmit="return sendForm(this)">
    <div class="form-group row">
        <label asp-for="IsManly" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <input asp-for="IsManly" class="form-control" type="checkbox">
        </div>
    </div>

    <div class="form-group row">
        <label asp-for="Name" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <input asp-for="Name" class="form-control">
            <span asp-validation-for="Name"></span>
        </div>
    </div>

    <button class="btn btn-primary">Send</button>
    <button type="button" class="btn btn-primary" onclick="return sendForm(this)">Send w/o client validation</button>
</form>
```