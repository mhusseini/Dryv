using System;
using Dryv.Configuration;

namespace Dryv.Translation
{
    public class TranslationResult
    {
        public string CodeTemplate { get; set; }
        public Func<Func<Type, object>, object[], DryvOptions, string> Factory { get; set; }
        public Type[] InjectedServiceTypes { get; set; }
    }
}