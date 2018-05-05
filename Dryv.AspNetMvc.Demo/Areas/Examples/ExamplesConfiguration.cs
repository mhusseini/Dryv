using System.Web;

namespace DryvDemo.Areas.Examples
{
    public static class ExamplesConfiguration
    {
        public static IOptions<TOptions> CreateOptionsFromCookie<TOptions>()
        where TOptions : new()
        {
            var request = HttpContext.Current.Request;
            var cookie = request.Cookies.Get(typeof(TOptions).Name);
            var option = cookie == null
                ? new TOptions()
                : Newtonsoft.Json.JsonConvert.DeserializeObject<TOptions>(cookie.Value);
            return new Options<TOptions>(option);
        }
    }
}