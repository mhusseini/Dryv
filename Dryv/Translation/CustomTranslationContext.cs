using System.Linq.Expressions;

namespace Dryv.Translation
{
    public class CustomTranslationContext : TranslationContext
    {
        public Expression Expression { get; set; }

        public bool Negated { get; set; }
    }
}