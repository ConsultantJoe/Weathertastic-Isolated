using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using Weathertastic_Isolated.Temperature.Infrastructure.Services;

namespace Weathertastic_Isolated.Temperature.Api
{
    public class Startup
    {
        public IHostApplicationLifetime ApplicationLifetime { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup))
                .AddHttpClient()
                .AddScoped<IWeatherService, WeatherService>()
                .AddSingleton(Log.Logger);
                
            ConfigureSwagger(services);
            ConfigureInfrastructure(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            ApplicationLifetime = applicationLifetime;
            
            CheckDependencies();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(s => 
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Weathertastic_Isolated.Temperature.Api");
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
                endpoints.MapControllers();
            });
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Weathertastic_Isolated.Temperature.Api", 
                    Version = "v1" });
            });
        }
        private void CheckDependencies()
        {
            var configVariableNames = new List<string>
            {
                "WEATHER_SERVICE_BASE_URI",
                "WEATHER_SERVICE_API_KEY",
                "WEATHER_SERVICE_MAX_RETRY_ATTEMPTS",
                "WEATHER_SERVICE_LENGTH_BETWEEN_RETRIES"
            };

            foreach (var configVariable in configVariableNames)
            {
                var variableName = Configuration[configVariable];

                if (string.IsNullOrEmpty(variableName))
                {
                    Log.Logger.Fatal($"Startup::CheckDependencies - The Configuration Variable {variableName} is null. Please populate the variable.  API is stopping.");
                    ApplicationLifetime.StopApplication();
                }
            }
        }

        private void ConfigureInfrastructure(IServiceCollection services)
        {
            services.AddSingleton(new WeatherServiceSettings
            {
                WeatherServiceApiKey = Configuration["WEATHER_SERVICE_API_KEY"],
                WeatherServiceBaseUri = Configuration["WEATHER_SERVICE_BASE_URI"],
                WeatherServiceLengthBetweenRetries = TimeSpan.FromSeconds(int.Parse(Configuration["WEATHER_SERVICE_LENGTH_BETWEEN_RETRIES"])),
                WeatherServiceMaxRetryAttempts = int.Parse(Configuration["WEATHER_SERVICE_MAX_RETRY_ATTEMPTS"])
            });
        }


    }
}
