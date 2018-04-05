namespace Dryv.Demo.Models
{
    public class Model6
    {
        public static readonly DryvRules Rules = DryvRules
            .For<Model6>()
            .Rule(m => m.Name, m => string.IsNullOrWhiteSpace(m.Name)
                ? "Name must not be empty"
                : null);

        [DryvRules]
        public string Name { get; set; }
    }
}