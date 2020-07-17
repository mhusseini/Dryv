using System.Text.Json;
using System.Text.Json.Serialization;
using Dryv.AspNetCore;
using Dryv.SampleVue.CustomValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseDryv();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services
                .AddMvc(options => { options.EnableEndpointRouting = true; })
                .AddRazorRuntimeCompilation()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                })
                .AddDryv()
                .AddDryvDynamicControllers(options => options.GeneratedAssemblyOutput = ass => new Lokad.ILPack.AssemblyGenerator().GenerateAssembly(ass, ass.GetName().Name + ".dll"))
            //.AddDryvPreloading()
            ;

            services.AddSingleton<AsyncValidator>();
            services.AddSingleton<SyncValidator>();
            services.AddOptions<SampleOptions>();
        }
    }
}