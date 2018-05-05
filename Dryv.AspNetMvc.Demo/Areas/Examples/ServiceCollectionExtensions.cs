namespace DryvDemo.Areas.Examples
{
    public interface IOptions<out TOptions>
    {
        TOptions Value { get; }
    }

    public class Options<TOptions> : IOptions<TOptions>
    {
        public TOptions Value { get; }

        public Options(TOptions value) => this.Value = value;
    }
}