using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dryv.AspNetCore;
using Dryv.AspNetCore.DynamicControllers;
using Dryv.Configuration;
using Dryv.SampleVue.CustomValidation;
using Dryv.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dryv.SampleVue
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "home",
                    "/",
                    new { controller = "Home", action = "Index" });
            });

            app.UseDryv();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AsyncValidator>();
            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = true;
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)))
                .AddDryv(options => options.UseClientFunctionWriter<DryvAsyncClientValidationFunctionWriter>())
                .AddDryvDynamicControllers()
                //.AddDryvPreloading()
                //.AddTranslator<AsyncValidatorTranslator>()
                ;

            services.AddRouting();
        }
    }
}