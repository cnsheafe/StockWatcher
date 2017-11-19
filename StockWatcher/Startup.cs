using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.PostgreSql;

using Npgsql;

using StockWatcher.Model;
using StockWatcher.Model.Services;
using StockWatcher.Model.Services.Helpers;

namespace StockWatcher
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services) 
        {
            string connectionString = Configuration.GetConnectionString("PostGresConnection");
            ServicesHelper(services, connectionString);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = ConnectionStringParser
                .Parse(Environment.GetEnvironmentVariable("DATABASE_URL"));
            ServicesHelper(services, connectionString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                Console.WriteLine("Is Development");
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(
                    new WebpackDevMiddlewareOptions
                    {
                        HotModuleReplacement = true,
                        ReactHotModuleReplacement = true
                    }
                );
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors(builder => {
                builder.AllowAnyOrigin();
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            // app.UseHangfireDashboard();
            app.UseHangfireServer(
                // Heroku Postgres has max connections of 20
                new BackgroundJobServerOptions{ WorkerCount = 5}
            );
        }

        private void ServicesHelper(IServiceCollection services, string connectionString) 
        {
            services.AddMvc();
            services.AddCors();

            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(connectionString)
            );


            services.AddDbContext<StockDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            services.AddScoped<IStockRequestService, StockRequestService>();
            services.AddTransient<IQueryCompanyService, QueryCompanyService>();
            services.AddTransient<AlphaVantage, AlphaVantageService>
            (avService => new AlphaVantageService(Environment.GetEnvironmentVariable("AV_KEY")));
        }

    }
}
