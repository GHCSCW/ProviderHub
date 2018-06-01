using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace BehavorialHealthWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthenticationCore();
            services.AddMvcCore(options =>
             {
                 options.RequireHttpsPermanent = true; // does not affect api requests
                 options.RespectBrowserAcceptHeader = true; // false by default

             })
             .AddFormatterMappings()
             .AddJsonFormatters();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("BehavorialHealthUser", policy => policy.RequireRole(@"GHC-HMO\App_BehavioralHealth_Provider_User"));
                options.AddPolicy("BehavorialHealthEditor", policy => policy.RequireRole(@"GHC-HMO\App_BehavioralHealth_Provider_Editor"));
                options.AddPolicy("BehavorialHealthSuperUser", policy => policy.RequireRole(@"GHC-HMO\App_BehavioralHealth_Super_User"));
                options.AddPolicy("BehavorialHealthAnonymous", policy => policy.RequireRole(@"GHC-HMO\App_BehavioralHealth_Anonymous"));

            });
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File(
            @"E:\logs\behavorialhealth.txt",
                fileSizeLimitBytes: 1_000_000,
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1))
                  .CreateLogger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

        }


    }
}
