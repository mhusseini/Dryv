using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Dryv.SampleVue
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-DE");
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-DE");
            CreateHostBuilder(args).Build().Run();
        }
    }
}