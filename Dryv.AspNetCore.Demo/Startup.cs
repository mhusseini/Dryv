using System.Linq;
using Dryv;
using DryvDemo.Controllers;
using DryvDemo.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
                .Use(async (context, next) =>
                {
                    if (context.Request.HasFormContentType)
                    {
                        if (context.Request.Form.TryGetValue(nameof(RuleSwitchExampleVieWModelOptions.CompanyNameRequired), out var value))
                        {
                            context.RequestServices.GetService<IOptions<RuleSwitchExampleVieWModelOptions>>().Value.CompanyNameRequired = value.Any(v => bool.TryParse(v, out var b) && b);
                        }

                        if (context.Request.Form.TryGetValue(nameof(InjectedObjectsExampleVieWModelOptions.CompanyPrefix), out value))
                        {
                            context.RequestServices.GetService<IOptions<InjectedObjectsExampleVieWModelOptions>>().Value.CompanyPrefix = value.FirstOrDefault();
                        }

                        if (context.Request.Form.TryGetValue(nameof(InjectedObjectsExampleVieWModelOptions.SloganPostfix), out value))
                        {
                            context.RequestServices.GetService<IOptions<InjectedObjectsExampleVieWModelOptions>>().Value.SloganPostfix = value.FirstOrDefault();
                        }
                    }

                    await next();
                })
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
            services.AddMvc().AddDryv();

            services.Configure<RuleSwitchExampleVieWModelOptions>(options =>
            {
                options.CompanyNameRequired = true;
            });

            services.Configure<InjectedObjectsExampleVieWModelOptions>(options =>
            {
                options.CompanyPrefix = "Awesome";
                options.SloganPostfix = "is cool";
            });
        }
    }
}