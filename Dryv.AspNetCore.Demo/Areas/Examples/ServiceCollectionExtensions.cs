using DryvDemo.Areas.Examples.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DryvDemo.Areas.Examples
{
    public static class ServiceCollectionExtensions
    {
        public static void AddExamples(this IServiceCollection services)
        {
            RegisterCookieOptions<Options2>(services);
            RegisterCookieOptions<Options3>(services);
        }

        private static void RegisterCookieOptions<T>(IServiceCollection services)
            where T : class, new()
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(s =>
            {
                var request = s.GetRequiredService<IHttpContextAccessor>().HttpContext.Request;
                var option = request.Cookies.TryGetValue(typeof(T).Name, out var json)
                    ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json)
                    : new T();
                return Options.Create(option);
            });
        }
    }
}