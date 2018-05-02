using System.Web;
using DryvDemo.Areas.Examples.Models;
using Unity;
using Unity.Injection;

namespace DryvDemo.Areas.Examples
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterExamples(this IUnityContainer services)
        {
            RegisterCookieOptions<Options2>(services);
            RegisterCookieOptions<Options3>(services);
        }

        private static void RegisterCookieOptions<T>(IUnityContainer services)
            where T : class, new()
        {
            services.RegisterType<IOptions<T>>(new InjectionFactory(s =>
            {
                var request = HttpContext.Current.Request;
                var cookie = request.Cookies.Get(typeof(T).Name);
                var option = cookie == null
                    ? new T()
                    : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cookie.Value);
                return new Options<T>(option);
            }));
        }
    }

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