using Dryv.Demo.Controllers;
using Dryv.Demo.Models;
using Dryv.Demo.Nav;
using Dryv.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv.Demo
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
                    routes.MapRoute(
                        name: string.Empty,
                        template: "{controller}/{action?}",
                        defaults: new { controller = "Home", action = nameof(HomeController.Index) });
                });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<NavCollector>();

            services.AddDryv();
            services.AddSingleton(Options.Create(new TenantOptions
            {
                IsEmptyCompanyAllowed = true
            }));
        }
    }
}