namespace Dryv
{
    public class ExpressionNotSupportedException : DryvException
    {
        public ExpressionNotSupportedException(string message)
            : base(message)
        {
        }
    }
}