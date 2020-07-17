using System.Collections.Generic;

namespace Dryv.Translation
{
    public class TranslatorProvider
    {
        public ICollection<ICustomTranslator> GenericTranslators { get; } = new SortedSet<ICustomTranslator>(CustomTranslatorComparer.Default);

        public ICollection<IMethodCallTranslator> MethodCallTranslators { get; } = new SortedSet<IMethodCallTranslator>(MethodCallTranslatorComparer.Default);

        private class CustomTranslatorComparer : IComparer<ICustomTranslator>
        {
            public static readonly CustomTranslatorComparer Default = new CustomTranslatorComparer();

            public int Compare(ICustomTranslator x, ICustomTranslator y)
            {
                var r = (x?.OrderIndex ?? 0).CompareTo(y?.OrderIndex ?? 0);
                return r == 0 ? 1 : r;
            }
        }

        private class MethodCallTranslatorComparer : IComparer<IMethodCallTranslator>
        {
            public static readonly MethodCallTranslatorComparer Default = new MethodCallTranslatorComparer();

            public int Compare(IMethodCallTranslator x, IMethodCallTranslator y)
            {
                var r = (x?.OrderIndex ?? 0).CompareTo(y?.OrderIndex ?? 0);
                return r == 0 ? 1 : r;
            }
        }
    }
}