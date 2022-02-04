using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.HostedServices;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Predications.InflationPredicating;
using MoneyCheckWebApp.Services;

namespace MoneyCheckWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = 
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml" });
            });
            
#if !REMOTE
            services.AddDbContext<MoneyCheckDbContext>(x =>
                x.UseLazyLoadingProxies()
                    .LogTo(System.Console.WriteLine,
                        (eventId, logLevel) => logLevel > LogLevel.Information
                                               || eventId == RelationalEventId.CommandExecuting)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .UseSqlServer(Configuration.GetConnectionString("MoneyCheckDb")));
#else
            services.AddDbContext<MoneyCheckDbContext>(x =>
                x.UseLazyLoadingProxies()
                 .LogTo(System.Console.WriteLine,
                          (eventId, logLevel) => logLevel > LogLevel.Information
                                              || eventId == RelationalEventId.CommandExecuting)
                 .EnableSensitiveDataLogging()
                 .EnableDetailedErrors()
                 .UseSqlServer(Configuration.GetConnectionString("MoneyCheckDbRemote")));
#endif
            
            services.AddHostedService<AuthorizationTokenLifetimeEnvironmentService>();
            services.AddHostedService<NeuralNetworkWeightsActualizerHostedService>();

            services.AddTransient<AuthorizationService>();
            services.AddTransient<InflationPredicationProcessor>();
            services.AddTransient<CookieService>();
            services.AddControllersWithViews();
            services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = "swagger";
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseCookieAuthorizationMiddleware(); //ПО промежуточного слоя по Cookie
            app.UseTokenAuthorizationMiddleware(); //ПО промежуточного слоя по токену
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}