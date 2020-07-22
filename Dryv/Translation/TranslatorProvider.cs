using System.Collections.Generic;

namespace Dryv.Translation
{
    public class TranslatorProvider
    {
        public ICollection<IDryvCustomTranslator> GenericTranslators { get; } = new SortedSet<IDryvCustomTranslator>(CustomTranslatorComparer.Default);

        public ICollection<IDryvMethodCallTranslator> MethodCallTranslators { get; } = new SortedSet<IDryvMethodCallTranslator>(MethodCallTranslatorComparer.Default);

        private class CustomTranslatorComparer : IComparer<IDryvCustomTranslator>
        {
            public static readonly CustomTranslatorComparer Default = new CustomTranslatorComparer();

            public int Compare(IDryvCustomTranslator x, IDryvCustomTranslator y)
            {
                var r = (x?.OrderIndex ?? 0).CompareTo(y?.OrderIndex ?? 0);
                return r == 0 ? 1 : r;
            }
        }

        private class MethodCallTranslatorComparer : IComparer<IDryvMethodCallTranslator>
        {
            public static readonly MethodCallTranslatorComparer Default = new MethodCallTranslatorComparer();

            public int Compare(IDryvMethodCallTranslator x, IDryvMethodCallTranslator y)
            {
                var r = (x?.OrderIndex ?? 0).CompareTo(y?.OrderIndex ?? 0);
                return r == 0 ? 1 : r;
            }
        }
    }
}