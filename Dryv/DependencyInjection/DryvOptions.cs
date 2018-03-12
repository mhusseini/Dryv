using System.Collections.Generic;
using Dryv.MethodCallTranslation;

namespace Dryv.DependencyInjection
{
    public class DryvOptions
    {
        public List<IGenericTranslator> GenericTanslators { get; } = new List<IGenericTranslator>();

        public List<IMethodCallTranslator> MethodCallTanslators { get; } = new List<IMethodCallTranslator>
        {
            new StringMethodCallTranslator(),
            new RegexMethodCallTranslator()
        };

        public bool PrettyPrintJavaScript { get; set; }
    }
}