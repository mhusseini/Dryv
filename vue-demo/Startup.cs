using Dryv;
using DryvDemo.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DryvDemo
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
            services.AddSingleton<ZipCodeValidator>();
            services.AddMvc()
                .AddDryv();

            services.Configure<DemoValidationOptions>(options =>
            {
                options.IsAddressRequired = true;
            });
        }
    }
}