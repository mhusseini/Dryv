using Dryv;
using DryvDemo.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DryvDemo
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } };

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseStaticFiles()
                .UseDryv()
                .UseMvc(routes =>
                {
                    routes
                        .MapRoute(
                        name: string.Empty,
                        template: "{controller}/{action?}",
                        defaults: new { controller = "Home", action = nameof(HomeController.Index) });
                });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddDryv();

            services.AddSingleton<ZipCodeValidator>();
            services.Configure<DemoValidationOptions>(options => options.IsAddressRequired = false);
        }
    }
}