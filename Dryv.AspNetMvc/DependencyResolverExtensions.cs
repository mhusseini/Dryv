using System.Web.Mvc;
using Dryv.Translation;
using Dryv.Utils;

namespace Dryv.AspNetMvc
{
    public static class DependencyResolverExtensions
    {
        public static void StartDryv(this IDependencyResolver resolver)
        {
            var translatorProvider = resolver.GetService<ITranslatorProvider>();

            translatorProvider.MethodCallTranslators.AddRange(resolver.GetServices<IMethodCallTranslator>());
            translatorProvider.GenericTranslators.AddRange(resolver.GetServices<ICustomTranslator>());

        }
    }
}